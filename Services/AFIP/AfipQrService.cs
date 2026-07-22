using System.Globalization;
using System.Text;
using System.Text.Json;
using QRCoder;

namespace FiyiPOS.Services.AFIP
{
    public class AfipQrService
    {
        public const string QrBaseUrl = "https://www.afip.gob.ar/fe/qr/";

        public string BuildQrUrl(AfipQrComprobanteData data)
        {
            var json = JsonSerializer.Serialize(new
            {
                ver = 1,
                fecha = data.Fecha.ToString("yyyy-MM-dd"),
                cuit = data.CuitEmisor,
                ptoVta = data.PuntoVenta,
                tipoCmp = data.TipoComprobante,
                nroCmp = data.NumeroComprobante,
                importe = data.Importe,
                moneda = data.Moneda,
                ctz = data.Cotizacion,
                tipoDocRec = data.TipoDocReceptor,
                nroDocRec = data.NroDocReceptor,
                tipoCodAut = data.TipoCodigoAutorizacion,
                codAut = data.CodigoAutorizacion
            });

            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            return $"{QrBaseUrl}?p={base64}";
        }

        public string GenerateQrImageBase64(string qrUrl, int pixelsPerModule = 4)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.M);
            var qrCode = new PngByteQRCode(data);
            byte[] png = qrCode.GetGraphic(pixelsPerModule);
            return Convert.ToBase64String(png);
        }

        public (string Url, string ImageBase64) Generate(AfipQrComprobanteData data, int pixelsPerModule = 4)
        {
            var url = BuildQrUrl(data);
            var image = GenerateQrImageBase64(url, pixelsPerModule);
            return (url, image);
        }

        public long ResolveCuitEmisor(string? cuitConfig)
        {
            var cuit = AfipCertificateHelper.NormalizeCuit(cuitConfig);
            if (cuit.Length == 11 && long.TryParse(cuit, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
                return n;
            return 20999999999L;
        }

        public long ParseCodigoAutorizacion(string cae)
        {
            var digits = new string(cae.Where(char.IsDigit).ToArray());
            return long.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0L;
        }
    }

    public class AfipQrComprobanteData
    {
        public DateTime Fecha { get; init; }
        public long CuitEmisor { get; init; }
        public int PuntoVenta { get; init; }
        public int TipoComprobante { get; init; }
        public long NumeroComprobante { get; init; }
        public decimal Importe { get; init; }
        public string Moneda { get; init; } = "PES";
        public int Cotizacion { get; init; } = 1;
        public int TipoDocReceptor { get; init; }
        public long NroDocReceptor { get; init; }
        public string TipoCodigoAutorizacion { get; init; } = "E";
        public long CodigoAutorizacion { get; init; }
    }
}
