namespace FiyiPOS.Services.AFIP.DTOs
{
    public class CertificateInformationDTO
    {
        public bool IsValid { get; set; }
        public string? Subject { get; set; }
        public string? Issuer { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Error { get; set; }
    }
}
