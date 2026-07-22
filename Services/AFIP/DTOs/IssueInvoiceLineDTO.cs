namespace FiyiPOS.Services.AFIP.DTOs
{
    public class IssueInvoiceLineDTO
    {
        public string Description { get; set; } = "";
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}
