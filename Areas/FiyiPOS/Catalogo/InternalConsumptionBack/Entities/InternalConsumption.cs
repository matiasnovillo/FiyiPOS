using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Entities
{
    public partial class InternalConsumption
    {
        [Key]
        public long InternalConsumptionId { get; set; }

        public long StoreId { get; set; }

        public long SystemUserId { get; set; }

        public DateTime ConsumptionDate { get; set; }

        public string Reason { get; set; } = string.Empty;

        public decimal TotalCost { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
