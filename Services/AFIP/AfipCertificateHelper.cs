using System.Security.Cryptography.X509Certificates;
using FiyiPOS.Services.AFIP.DTOs;

namespace FiyiPOS.Services.AFIP
{
    public static class AfipCertificateHelper
    {
        public const int MaxPfxSizeBytes = 512 * 1024;

        public static X509Certificate2 LoadCertificate(byte[] pfxBytes, string password)
        {
            return X509CertificateLoader.LoadPkcs12(pfxBytes, password, X509KeyStorageFlags.EphemeralKeySet);
        }

        public static CertificateInformationDTO ValidateCertificate(byte[] pfxBytes, string password)
        {
            try
            {
                using var cert = LoadCertificate(pfxBytes, password);
                return new CertificateInformationDTO
                {
                    IsValid = true,
                    Subject = cert.Subject,
                    Issuer = cert.Issuer,
                    ExpirationDate = cert.NotAfter
                };
            }
            catch (Exception ex)
            {
                return new CertificateInformationDTO
                {
                    IsValid = false,
                    Error = ex.Message
                };
            }
        }

        public static string NormalizeCuit(string? cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit)) return "";
            return new string(cuit.Where(char.IsDigit).ToArray());
        }
    }
}
