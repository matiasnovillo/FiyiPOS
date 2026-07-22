using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Entities
{
    public partial class CashMovement
    {
        [Key]
        public long     CashMovementId { get; set; }
        public long     StoreId        { get; set; }
        public long     SystemUserId   { get; set; }
        public string   Type           { get; set; } = string.Empty; // "IN" | "OUT"
        public decimal  Amount         { get; set; }
        public string   Reason         { get; set; } = string.Empty;
        public DateTime MovementDate   { get; set; }
    }
}
