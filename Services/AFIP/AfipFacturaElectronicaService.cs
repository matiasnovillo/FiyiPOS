using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Repositories;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Repositories;
using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Repositories;
using FiyiPOS.Services.AFIP.DTOs;

namespace FiyiPOS.Services.AFIP
{
    public class AfipFacturaElectronicaService(
        CompanySettingsRepository companySettingsRepository,
        AfipTokenService tokenService,
        AfipWsfeClient wsfeClient,
        InvoiceRepository invoiceRepository,
        AfipLogRepository afipLogRepository,
        AfipQrService afipQrService,
        ILogger<AfipFacturaElectronicaService> logger)
    {
        public async Task<IssueInvoiceResultDTO> EmitirFacturaAsync(
            IssueInvoiceRequestDTO request, CancellationToken ct = default)
        {
            var config = await companySettingsRepository.GetByStoreIdAsync(request.StoreId);
            if (config is null || !config.InvoicingEnabled)
            {
                return new IssueInvoiceResultDTO
                {
                    IsSuccess = false,
                    UserMessage = "La facturación electrónica no está habilitada."
                };
            }

            bool esHomologacion = config.Environment == AfipConstants.AmbienteHomologacion;
            bool tieneCertificado = config.CertificatePfx is { Length: > 0 }
                                 && !string.IsNullOrEmpty(config.CertificatePassword);

            if (!esHomologacion && !tieneCertificado)
            {
                return new IssueInvoiceResultDTO
                {
                    IsSuccess = false,
                    UserMessage = "En producción es obligatorio cargar el certificado .pfx."
                };
            }

            if (!esHomologacion && string.IsNullOrEmpty(config.TaxId))
            {
                return new IssueInvoiceResultDTO
                {
                    IsSuccess = false,
                    UserMessage = "El CUIT es obligatorio en producción."
                };
            }

            if (tieneCertificado && config.ExpirationDate.HasValue && config.ExpirationDate.Value < DateTime.Now)
            {
                return new IssueInvoiceResultDTO
                {
                    IsSuccess = false,
                    UserMessage = "El certificado AFIP está vencido. Cargue uno nuevo en Configuración."
                };
            }

            // Homologación sin certificado: ticket de prueba local (no llama a AFIP)
            if (esHomologacion && !tieneCertificado)
                return await EmitirFacturaSimuladaAsync(config, request, ct);

            try
            {
                if (string.IsNullOrEmpty(config.TaxId))
                {
                    return new IssueInvoiceResultDTO
                    {
                        IsSuccess = false,
                        UserMessage = "El CUIT es obligatorio para conectar con AFIP."
                    };
                }

                var (token, sign) = await tokenService.GetValidCredentialsAsync(config, ct);
                long cuit = long.Parse(AfipCertificateHelper.NormalizeCuit(config.TaxId));
                int tipoComprobante = DeterminarTipoComprobante(config, request.ClientCUIT);
                var (docTipo, docNro) = DeterminarDocumentoCliente(request.ClientCUIT);

                long ultimo = await wsfeClient.GetUltimoComprobanteAsync(
                    cuit, token, sign, config.Environment, config.PointOfSale, tipoComprobante, ct);
                long numero = ultimo + 1;

                bool esFacturaC = tipoComprobante == AfipConstants.FacturaC;
                decimal impNeto = esFacturaC ? request.Total : request.Subtotal;
                decimal impIva = esFacturaC ? 0 : request.TaxAmount;
                int ivaId = AfipConstants.MapIvaId(request.TaxRate);
                bool incluirIva = !esFacturaC && impIva > 0;

                var (resultado, cae, caeVto, rawResponse, errores) = await wsfeClient.SolicitarCaeAsync(
                    cuit, token, sign, config.Environment,
                    config.PointOfSale, tipoComprobante, numero,
                    request.Concept, docTipo, docNro, DateTime.Now,
                    request.Total, impNeto, impIva,
                    ivaId, impNeto, impIva, incluirIva, ct);

                var invoiceNumber = $"{config.PointOfSale:D4}-{numero:D8}";
                var exito = resultado == "A" && !string.IsNullOrEmpty(cae);

                var invoice = new Invoice
                {
                    SaleId = request.SaleId,
                    StoreId = request.StoreId,
                    ClientId = request.ClientId,
                    SystemUserId = request.SystemUserId,
                    InvoiceNumber = invoiceNumber,
                    DocumentType = tipoComprobante,
                    PointOfSale = config.PointOfSale,
                    DocumentNumber = numero,
                    CAE = cae,
                    CAEExpirationDate = caeVto,
                    Subtotal = request.Subtotal,
                    TaxAmount = request.TaxAmount,
                    Total = request.Total,
                    Status = exito ? "active" : "error",
                    AfipResult = resultado,
                    AfipErrors = errores.Count > 0 ? string.Join(" | ", errores) : null,
                    Environment = config.Environment,
                    XmlResponse = rawResponse
                };
                await invoiceRepository.AddAsync(invoice);

                await afipLogRepository.AddAsync(new AfipLog
                {
                    StoreId = request.StoreId,
                    Operation = "FECAESolicitar",
                    Response = rawResponse,
                    IsError = !exito,
                    ErrorMessage = exito ? null : string.Join(" | ", errores)
                });

                if (!exito)
                {
                    logger.LogWarning("AFIP rechazó comprobante para venta {SaleId}: {Errores}",
                        request.SaleId, string.Join(" | ", errores));
                }

                var result = new IssueInvoiceResultDTO
                {
                    IsSuccess = exito,
                    CAE = cae,
                    CAEExpirationDate = caeVto,
                    ReceiptNumber = invoiceNumber,
                    ReceiptType = tipoComprobante,
                    PointOfSale = config.PointOfSale,
                    NumericReceiptNumber = numero,
                    Result = resultado,
                    Errors = errores,
                    UserMessage = exito
                        ? $"Factura {invoiceNumber} autorizada. CAE: {cae}"
                        : $"AFIP no autorizó la factura: {string.Join(" | ", errores)}"
                };
                AgregarQrAlResultado(result, config, request, DateTime.Now, docTipo, docNro);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al emitir factura para venta {SaleId}", request.SaleId);
                await afipLogRepository.AddAsync(new AfipLog
                {
                    StoreId = request.StoreId,
                    Operation = "FECAESolicitar",
                    IsError = true,
                    ErrorMessage = ex.Message
                });

                return new IssueInvoiceResultDTO
                {
                    IsSuccess = false,
                    Errors = [ex.Message],
                    UserMessage = $"Error al facturar: {ex.Message}"
                };
            }
        }

        private static int DeterminarTipoComprobante(CompanySettings config, string clienteCuit)
        {
            if (config.TaxCondition.Equals("Monotributista", StringComparison.OrdinalIgnoreCase))
                return AfipConstants.FacturaC;

            var cuit = AfipCertificateHelper.NormalizeCuit(clienteCuit);
            return cuit.Length == 11 ? AfipConstants.FacturaA : AfipConstants.FacturaB;
        }

        private static (int DocTipo, long DocNro) DeterminarDocumentoCliente(string clienteCuit)
        {
            var cuit = AfipCertificateHelper.NormalizeCuit(clienteCuit);
            if (cuit.Length == 11)
                return (AfipConstants.DocTipoCuit, long.Parse(cuit));
            if (cuit.Length is >= 7 and <= 8)
                return (AfipConstants.DocTipoDni, long.Parse(cuit));
            return (AfipConstants.DocTipoConsumidorFinal, 0);
        }

        private async Task<IssueInvoiceResultDTO> EmitirFacturaSimuladaAsync(
            CompanySettings config, IssueInvoiceRequestDTO request, CancellationToken ct)
        {
            int tipoComprobante = DeterminarTipoComprobante(config, request.ClientCUIT);
            long ultimo = await invoiceRepository.GetUltimoNumeroComprobanteAsync(
                request.StoreId, config.Environment, config.PointOfSale, tipoComprobante);
            long numero = ultimo + 1;
            var invoiceNumber = $"{config.PointOfSale:D4}-{numero:D8}";
            var caeVto = DateTime.Now.AddDays(10);
            var cae = $"7{request.SaleId:D13}"[..14];

            var invoice = new Invoice
            {
                SaleId = request.SaleId,
                StoreId = request.StoreId,
                ClientId = request.ClientId,
                SystemUserId = request.SystemUserId,
                InvoiceNumber = invoiceNumber,
                DocumentType = tipoComprobante,
                PointOfSale = config.PointOfSale,
                DocumentNumber = numero,
                CAE = cae,
                CAEExpirationDate = caeVto,
                Subtotal = request.Subtotal,
                TaxAmount = request.TaxAmount,
                Total = request.Total,
                Status = "demo",
                AfipResult = "S",
                Environment = config.Environment,
                XmlResponse = "Comprobante simulado — homologación sin certificado AFIP"
            };
            await invoiceRepository.AddAsync(invoice);

            await afipLogRepository.AddAsync(new AfipLog
            {
                StoreId = request.StoreId,
                Operation = "DEMO_Homologacion",
                Response = $"Comprobante simulado {invoiceNumber}, CAE demo {cae}",
                IsError = false
            });

            logger.LogInformation("Comprobante de prueba {InvoiceNumber} para venta {SaleId}", invoiceNumber, request.SaleId);

            var (docTipo, docNro) = DeterminarDocumentoCliente(request.ClientCUIT);
            var result = new IssueInvoiceResultDTO
            {
                IsSuccess = true,
                IsSimulated = true,
                CAE = cae,
                CAEExpirationDate = caeVto,
                ReceiptNumber = invoiceNumber,
                ReceiptType = tipoComprobante,
                PointOfSale = config.PointOfSale,
                NumericReceiptNumber = numero,
                Result = "S",
                UserMessage = $"Ticket de prueba {invoiceNumber} (homologación, sin AFIP)."
            };
            AgregarQrAlResultado(result, config, request, DateTime.Now, docTipo, docNro);
            return result;
        }

        private void AgregarQrAlResultado(
            IssueInvoiceResultDTO result,
            CompanySettings config,
            IssueInvoiceRequestDTO request,
            DateTime fecha,
            int docTipo,
            long docNro)
        {
            if (!result.IsSuccess || string.IsNullOrEmpty(result.CAE))
                return;

            var qrData = new AfipQrComprobanteData
            {
                Fecha = fecha,
                CuitEmisor = afipQrService.ResolveCuitEmisor(config.TaxId),
                PuntoVenta = result.PointOfSale,
                TipoComprobante = result.ReceiptType,
                NumeroComprobante = result.NumericReceiptNumber,
                Importe = request.Total,
                TipoDocReceptor = docTipo,
                NroDocReceptor = docNro,
                CodigoAutorizacion = afipQrService.ParseCodigoAutorizacion(result.CAE)
            };

            var (url, image) = afipQrService.Generate(qrData);
            result.QrUrl = url;
            result.QrImageBase64 = image;
        }
    }
}
