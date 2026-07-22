using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Entities
{
    public partial class CashCut
    {
        [Key]
        public long CashCutId { get; set; }

        public long StoreId { get; set; }

        public long SystemUserId { get; set; }

        public decimal OpeningAmount { get; set; }

        public decimal ExpectedAmount { get; set; }

        public decimal ActualAmount { get; set; }

        public decimal Difference { get; set; }

        public int TotalSales { get; set; }

        public DateTime OpenedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public string? Notes { get; set; }
    }
}
