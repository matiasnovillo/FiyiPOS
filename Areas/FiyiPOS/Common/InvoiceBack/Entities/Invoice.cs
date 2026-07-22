namespace FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Entities
{
    public class Invoice
    {
        public long InvoiceId { get; set; }
        public long SaleId { get; set; }
        public long StoreId { get; set; }
        public long? ClientId { get; set; }
        public long SystemUserId { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public int DocumentType { get; set; }
        public int PointOfSale { get; set; }
        public long DocumentNumber { get; set; }
        public string? CAE { get; set; }
        public DateTime? CAEExpirationDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "active";
        public string? AfipResult { get; set; }
        public string? AfipErrors { get; set; }
        public string Environment { get; set; } = "Homologacion";
        public string? XmlRequest { get; set; }
        public string? XmlResponse { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
