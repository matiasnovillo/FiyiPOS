namespace FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.DTOs
{
    public class CashMovementListItem
    {
        public long     CashMovementId { get; set; }
        public string   Type           { get; set; } = "";
        public decimal  Amount         { get; set; }
        public string   Reason         { get; set; } = "";
        public DateTime MovementDate   { get; set; }
        public string   OperatorName   { get; set; } = "";
    }
}
