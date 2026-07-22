using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionDetailBack.Entities
{
    public partial class InternalConsumptionDetail
    {
        [Key]
        public long InternalConsumptionDetailId { get; set; }

        public long InternalConsumptionId { get; set; }

        public long ProductId { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal Subtotal { get; set; }
    }
}
