namespace FiyiPOS.Services.AFIP.DTOs
{
    public class IssueInvoiceRequestDTO
    {
        public long StoreId { get; set; }
        public long SaleId { get; set; }
        public long SystemUserId { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; } = "";
        public string ClientCUIT { get; set; } = "";
        public int Concept { get; set; } = 1;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public decimal TaxRate { get; set; }
        public List<IssueInvoiceLineDTO> Productos { get; set; } = new();
    }
}
