using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace FiyiPOS.Services.AFIP
{
    public class AfipWsfeClient(IHttpClientFactory httpClientFactory, ILogger<AfipWsfeClient> logger)
    {
        public async Task<long> GetUltimoComprobanteAsync(
            long cuit, string token, string sign, string ambiente, int ptoVta, int cbteTipo, CancellationToken ct = default)
        {
            var soap = BuildSoap("FECompUltimoAutorizado", $"""
                <ar:Auth>
                  <ar:Token>{Escape(token)}</ar:Token>
                  <ar:Sign>{Escape(sign)}</ar:Sign>
                  <ar:Cuit>{cuit}</ar:Cuit>
                </ar:Auth>
                <ar:PtoVta>{ptoVta}</ar:PtoVta>
                <ar:CbteTipo>{cbteTipo}</ar:CbteTipo>
                """);

            var body = await SendAsync(soap, ambiente, "FECompUltimoAutorizado", ct);
            var cbteNro = XDocument.Parse(body).Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "CbteNro")?.Value;

            if (!long.TryParse(cbteNro, out var nro))
                throw new InvalidOperationException("No se pudo obtener el último comprobante autorizado.");

            return nro;
        }

        public async Task<(string Resultado, string? Cae, DateTime? CaeVto, string RawResponse, List<string> Errores)>
            SolicitarCaeAsync(
                long cuit, string token, string sign, string ambiente,
                int ptoVta, int cbteTipo, long cbteNro, int concepto,
                int docTipo, long docNro, DateTime fecha,
                decimal impTotal, decimal impNeto, decimal impIva,
                int ivaId, decimal ivaBase, decimal ivaImporte,
                bool incluirIva,
                CancellationToken ct = default)
        {
            string fechaStr = fecha.ToString("yyyyMMdd");
            string ivaXml = incluirIva
                ? $"""
                  <ar:Iva>
                    <ar:AlicIva>
                      <ar:Id>{ivaId}</ar:Id>
                      <ar:BaseImp>{FormatAmount(ivaBase)}</ar:BaseImp>
                      <ar:Importe>{FormatAmount(ivaImporte)}</ar:Importe>
                    </ar:AlicIva>
                  </ar:Iva>
                  """
                : "";

            var detReq = $"""
                <ar:FECAEDetRequest>
                  <ar:Concepto>{concepto}</ar:Concepto>
                  <ar:DocTipo>{docTipo}</ar:DocTipo>
                  <ar:DocNro>{docNro}</ar:DocNro>
                  <ar:CbteDesde>{cbteNro}</ar:CbteDesde>
                  <ar:CbteHasta>{cbteNro}</ar:CbteHasta>
                  <ar:CbteFch>{fechaStr}</ar:CbteFch>
                  <ar:ImpTotal>{FormatAmount(impTotal)}</ar:ImpTotal>
                  <ar:ImpTotConc>0</ar:ImpTotConc>
                  <ar:ImpNeto>{FormatAmount(impNeto)}</ar:ImpNeto>
                  <ar:ImpOpEx>0</ar:ImpOpEx>
                  <ar:ImpTrib>0</ar:ImpTrib>
                  <ar:ImpIVA>{FormatAmount(impIva)}</ar:ImpIVA>
                  <ar:MonId>PES</ar:MonId>
                  <ar:MonCotiz>1</ar:MonCotiz>
                  {ivaXml}
                </ar:FECAEDetRequest>
                """;

            var soap = BuildSoap("FECAESolicitar", $"""
                <ar:Auth>
                  <ar:Token>{Escape(token)}</ar:Token>
                  <ar:Sign>{Escape(sign)}</ar:Sign>
                  <ar:Cuit>{cuit}</ar:Cuit>
                </ar:Auth>
                <ar:FeCAEReq>
                  <ar:FeCabReq>
                    <ar:CantReg>1</ar:CantReg>
                    <ar:PtoVta>{ptoVta}</ar:PtoVta>
                    <ar:CbteTipo>{cbteTipo}</ar:CbteTipo>
                  </ar:FeCabReq>
                  <ar:FeDetReq>
                    {detReq}
                  </ar:FeDetReq>
                </ar:FeCAEReq>
                """);

            var response = await SendAsync(soap, ambiente, "FECAESolicitar", ct);
            return ParseCaeResponse(response);
        }

        private async Task<string> SendAsync(string soap, string ambiente, string action, CancellationToken ct)
        {
            var client = httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, AfipConstants.WsfeUrl(ambiente))
            {
                Content = new StringContent(soap, Encoding.UTF8, "text/xml")
            };
            request.Headers.Add("SOAPAction", $"http://ar.gov.afip.dif.FEV1/{action}");

            logger.LogInformation("Invocando WSFE en ambiente {Ambiente}", ambiente);
            var response = await client.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"WSFE respondió HTTP {(int)response.StatusCode}: {body}");

            return body;
        }

        private static string BuildSoap(string action, string bodyContent)
        {
            return $"""
                <?xml version="1.0" encoding="utf-8"?>
                <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ar="http://ar.gov.afip.dif.FEV1/">
                  <soap:Body>
                    <ar:{action}>
                      {bodyContent}
                    </ar:{action}>
                  </soap:Body>
                </soap:Envelope>
                """;
        }

        private static (string Resultado, string? Cae, DateTime? CaeVto, string RawResponse, List<string> Errores)
            ParseCaeResponse(string soapBody)
        {
            var doc = XDocument.Parse(soapBody);
            var det = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "FECAEDetResponse");
            var resultado = det?.Elements().FirstOrDefault(e => e.Name.LocalName == "Resultado")?.Value ?? "";
            var cae = det?.Elements().FirstOrDefault(e => e.Name.LocalName == "CAE")?.Value;
            var caeVtoStr = det?.Elements().FirstOrDefault(e => e.Name.LocalName == "CAEFchVto")?.Value;

            DateTime? caeVto = null;
            if (!string.IsNullOrEmpty(caeVtoStr) &&
                DateTime.TryParseExact(caeVtoStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                caeVto = parsed;

            var errores = doc.Descendants()
                .Where(e => e.Name.LocalName is "Err" or "Obs")
                .SelectMany(e => e.Elements().Where(x => x.Name.LocalName is "Msg" or "Code")
                    .Select(x => $"{x.Name.LocalName}: {x.Value}"))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            var obs = doc.Descendants()
                .Where(e => e.Name.LocalName == "Observaciones")
                .SelectMany(e => e.Elements().Where(x => x.Name.LocalName == "Msg").Select(x => x.Value))
                .Where(s => !string.IsNullOrWhiteSpace(s));
            errores.AddRange(obs);

            return (resultado, cae, caeVto, soapBody, errores);
        }

        private static string FormatAmount(decimal amount) =>
            amount.ToString("0.00", CultureInfo.InvariantCulture);

        private static string Escape(string value) =>
            global::System.Security.SecurityElement.Escape(value) ?? value;
    }
}
