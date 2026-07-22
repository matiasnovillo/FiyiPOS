using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutDetailBack.Entities
{
    public partial class CashCutDetail
    {
        [Key]
        public long CashCutDetailId { get; set; }

        public long CashCutId { get; set; }

        public long PaymentMethodId { get; set; }

        public decimal TotalCollected { get; set; }

        public int TransactionCount { get; set; }
    }
}
