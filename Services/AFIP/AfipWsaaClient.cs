using System.Globalization;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace FiyiPOS.Services.AFIP
{
    public class AfipWsaaClient(IHttpClientFactory httpClientFactory, ILogger<AfipWsaaClient> logger)
    {
        public async Task<(string Token, string Sign, DateTime Expiration)> LoginAsync(
            byte[] pfxBytes, string password, string ambiente, string service, CancellationToken ct = default)
        {
            using var cert = AfipCertificateHelper.LoadCertificate(pfxBytes, password);
            var cms = SignLoginTicket(BuildLoginTicket(service), cert);
            var base64 = Convert.ToBase64String(cms);

            var soap = $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:wsaa="https://wsaahomo.afip.gov.ar/ws/services/LoginCms">
                   <soapenv:Header/>
                   <soapenv:Body>
                      <wsaa:loginCms>
                         <wsaa:in0>{base64}</wsaa:in0>
                      </wsaa:loginCms>
                   </soapenv:Body>
                </soapenv:Envelope>
                """;

            var client = httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, AfipConstants.WsaaUrl(ambiente))
            {
                Content = new StringContent(soap, Encoding.UTF8, "text/xml")
            };
            request.Headers.Add("SOAPAction", "");

            logger.LogInformation("Solicitando TA WSAA en ambiente {Ambiente}", ambiente);
            var response = await client.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"WSAA respondió HTTP {(int)response.StatusCode}: {body}");

            return ParseLoginResponse(body);
        }

        private static byte[] BuildLoginTicket(string service)
        {
            var now = DateTime.UtcNow.AddMinutes(-5);
            var exp = now.AddHours(12);
            var uniqueId = (uint)Random.Shared.Next(1, int.MaxValue);

            var xml = $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <loginTicketRequest version="1.0">
                  <header>
                    <uniqueId>{uniqueId}</uniqueId>
                    <generationTime>{now:yyyy-MM-ddTHH:mm:ss}</generationTime>
                    <expirationTime>{exp:yyyy-MM-ddTHH:mm:ss}</expirationTime>
                  </header>
                  <service>{service}</service>
                </loginTicketRequest>
                """;

            return Encoding.UTF8.GetBytes(xml);
        }

        private static byte[] SignLoginTicket(byte[] xmlBytes, X509Certificate2 cert)
        {
            var contentInfo = new ContentInfo(xmlBytes);
            var signedCms = new SignedCms(contentInfo);
            var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, cert)
            {
                IncludeOption = X509IncludeOption.EndCertOnly
            };
            signedCms.ComputeSignature(signer);
            return signedCms.Encode();
        }

        private static (string Token, string Sign, DateTime Expiration) ParseLoginResponse(string soapBody)
        {
            var doc = XDocument.Parse(soapBody);
            var loginCmsReturn = doc.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "loginCmsReturn")?.Value;

            if (string.IsNullOrWhiteSpace(loginCmsReturn))
                throw new InvalidOperationException("WSAA no devolvió loginCmsReturn.");

            var ticket = XDocument.Parse(loginCmsReturn);
            var token = ticket.Descendants().FirstOrDefault(e => e.Name.LocalName == "token")?.Value ?? "";
            var sign = ticket.Descendants().FirstOrDefault(e => e.Name.LocalName == "sign")?.Value ?? "";
            var expStr = ticket.Descendants().FirstOrDefault(e => e.Name.LocalName == "expirationTime")?.Value;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(sign))
                throw new InvalidOperationException("WSAA devolvió token o sign vacío.");

            DateTime expiration = DateTime.TryParse(expStr, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var exp)
                ? exp.ToLocalTime()
                : DateTime.Now.AddHours(11);

            return (token, sign, expiration);
        }
    }
}
