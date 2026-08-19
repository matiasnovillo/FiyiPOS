namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.DTOs
{
    public class CashCutReportDTO
    {
        public string   StoreName     { get; set; } = "";
        public string   OperatorName  { get; set; } = "";
        public DateTime GeneratedAt   { get; set; }

        public long     CashCutId     { get; set; }
        public DateTime OpenedAt      { get; set; }
        public DateTime ClosedAt      { get; set; }
        public decimal  OpeningAmount { get; set; }
        public decimal  ExpectedAmount{ get; set; }
        public decimal  ActualAmount  { get; set; }
        public decimal  Difference    { get; set; }
        public int      TotalSales    { get; set; }
        public decimal  TotalCOGS     { get; set; }
        public decimal  TotalSalesGross { get; set; }   // Ventas con IVA (Sale.Total)
        public decimal  TotalSalesNet   { get; set; }   // Ventas sin IVA (Sale.Total - Sale.TaxAmount)
        public string?  Notes         { get; set; }

        public List<PaymentLine>         PaymentLines       { get; set; } = new();
        public List<MovementLine>        Movements          { get; set; } = new();
        public List<SaleLine>            Sales              { get; set; } = new();
        public List<ReturnLine>          Returns            { get; set; } = new();
        public List<CreditLine>          Credits            { get; set; } = new();
        public List<ProductCategoryGroup> ProductsByCategory { get; set; } = new();
        public List<PurchaseLine>        Purchases          { get; set; } = new();
        public List<InternalConsumptionGroup> InternalConsumptionByOperator { get; set; } = new();

        public record PurchaseLine(string SupplierName, string? Invoice, DateTime Date, int ItemCount, decimal Subtotal, decimal TaxAmount, decimal Total, bool IsConfirmed);
        public record PaymentLine(string MethodName, bool IsCash, int Count, decimal Total);
        public record MovementLine(string Type, decimal Amount, string Reason, string Operator, DateTime Date, string PaymentMethodName);
        public record SaleLine(string Ticket, DateTime Date, string Operator, string Client, decimal Total, string PaymentMethod, bool IsCredit);
        public record ReturnLine(string OriginalTicket, DateTime Date, string Reason, decimal Amount, string Operator);
        public record CreditLine(string Client, decimal Original, decimal Balance, DateTime Date);
        public record ProductSoldLine(string ProductName, decimal TotalQty, decimal TotalSubtotal);
        public record PaymentMethodBreakdown(string MethodName, decimal Subtotal);
        public record ProductCategoryGroup(string CategoryName, List<ProductSoldLine> Lines, List<PaymentMethodBreakdown> PaymentBreakdown);
        public record ConsumptionLine(DateTime Date, string Reason, string ProductName, decimal Quantity, decimal Subtotal);
        public record InternalConsumptionGroup(string OperatorName, List<ConsumptionLine> Lines, decimal TotalCost);
    }
}
