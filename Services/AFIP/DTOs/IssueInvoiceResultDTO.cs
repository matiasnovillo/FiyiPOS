namespace FiyiPOS.Services.AFIP.DTOs
{
    public class IssueInvoiceResultDTO
    {
        public bool IsSuccess { get; set; }
        public string? CAE { get; set; }
        public DateTime? CAEExpirationDate { get; set; }
        public string? ReceiptNumber { get; set; }
        public int ReceiptType { get; set; }
        public int PointOfSale { get; set; }
        public long NumericReceiptNumber { get; set; }
        public string? Result { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? UserMessage { get; set; }
        public bool IsSimulated { get; set; }
        public string? QrUrl { get; set; }
        public string? QrImageBase64 { get; set; }
    }
}
