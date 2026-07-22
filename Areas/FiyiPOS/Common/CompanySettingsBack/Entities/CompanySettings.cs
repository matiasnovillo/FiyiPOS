namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities
{
    public class CompanySettings
    {
        public int Id { get; set; }
        public long StoreId { get; set; }
        public string CompanyName { get; set; } = "";
        public string LegalName { get; set; } = "";
        public string TaxId { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string TaxCondition { get; set; } = "Monotributista";
        public int PointOfSale { get; set; } = 1;
        public string Environment { get; set; } = "Homologacion";
        public bool InvoicingEnabled { get; set; }
        public byte[]? CertificatePfx { get; set; }
        public string? CertificatePassword { get; set; }
        public string? CertificateSubject { get; set; }
        public string? CertificateIssuer { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
