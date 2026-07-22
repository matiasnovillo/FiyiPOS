namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.DTOs
{
    public class CashCutPreviewDTO
    {
        public DateTime PeriodStart  { get; set; }
        public DateTime PeriodEnd    { get; set; }
        public int      SalesCount   { get; set; }
        public decimal  TotalRevenue { get; set; }
        public decimal  TotalCashIn  { get; set; }
        public decimal  TotalCashOut { get; set; }
        public List<CashCutDetailLineDTO> Lines { get; set; } = new();
    }

    public class CashCutDetailLineDTO
    {
        public long    PaymentMethodId   { get; set; }
        public string  PaymentMethodName { get; set; } = "";
        public bool    IsCash            { get; set; }
        public decimal TotalCollected    { get; set; }
        public int     TransactionCount  { get; set; }
    }
}
