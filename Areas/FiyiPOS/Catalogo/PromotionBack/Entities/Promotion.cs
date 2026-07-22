using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Entities
{
    public partial class Promotion
    {
        [Key]
        public long PromotionId { get; set; }

        public long StoreId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
