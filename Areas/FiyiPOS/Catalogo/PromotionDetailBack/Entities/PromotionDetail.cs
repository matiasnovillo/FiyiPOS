using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionDetailBack.Entities
{
    public partial class PromotionDetail
    {
        [Key]
        public long PromotionDetailId { get; set; }

        public long PromotionId { get; set; }

        public long ProductId { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
    }
}
