using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Entities
{
    public partial class SaleDetail
    {
        [Key]
        public long SaleDetailId { get; set; }

        public long SaleId { get; set; }

        public long? ProductId { get; set; }

        public string? BarcodeSnapshot { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal TaxRate { get; set; }

        public decimal Subtotal { get; set; }

        public decimal? CostPriceSnapshot { get; set; }

        public long? PromotionId { get; set; }
    }
}
