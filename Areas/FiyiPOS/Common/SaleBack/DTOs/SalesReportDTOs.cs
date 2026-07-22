namespace FiyiPOS.Areas.FiyiPOS.Common.SaleBack.DTOs
{
    public class SalesSummaryReport
    {
        public decimal TotalRevenue   { get; set; }   // Ventas con IVA (Sale.Total)
        public decimal TotalTaxAmount { get; set; }   // IVA total del período (Sale.TaxAmount)
        public decimal TotalRevenueNet => TotalRevenue - TotalTaxAmount;   // Ventas sin IVA
        public int     SalesCount    { get; set; }
        public decimal AverageTicket { get; set; }
        public List<PaymentMethodLine> ByPaymentMethod { get; set; } = new();
    }

    public class PaymentMethodLine
    {
        public string  MethodName { get; set; } = "";
        public int     Count      { get; set; }
        public decimal Total      { get; set; }
    }

    public class TopProductLine
    {
        public string  ProductName { get; set; } = "";
        public decimal UnitsSold   { get; set; }
        public decimal Revenue     { get; set; }
    }

    public class OperatorSalesLine
    {
        public string  OperatorName { get; set; } = "";
        public int     SalesCount   { get; set; }
        public decimal TotalSold    { get; set; }
    }

    public class SaleListLine
    {
        public long     SaleId       { get; set; }
        public string   TicketNumber { get; set; } = "";
        public DateTime SaleDate     { get; set; }
        public string   OperatorName { get; set; } = "";
        public string   ClientName   { get; set; } = "";
        public decimal  Total        { get; set; }
        public bool     IsCredit     { get; set; }
    }

    public class ProductCategoryGroup
    {
        public string                  CategoryName { get; set; } = "";
        public List<ProductCategoryLine> Lines      { get; set; } = new();
    }

    public class ProductCategoryLine
    {
        public string  ProductName   { get; set; } = "";
        public decimal TotalQty      { get; set; }
        public decimal TotalSubtotal { get; set; }
    }

    public class ProfitabilityLine
    {
        public string  ProductName  { get; set; } = "";
        public string  CategoryName { get; set; } = "";
        public decimal TotalQty     { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost    { get; set; }
        public decimal GrossProfit  { get; set; }
        public decimal MarginPct    { get; set; }
    }
}
