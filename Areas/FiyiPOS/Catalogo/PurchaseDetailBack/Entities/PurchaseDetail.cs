using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseDetailBack.Entities
{
    public partial class PurchaseDetail
    {
        [Key]
        public long PurchaseDetailId { get; set; }

        public long PurchaseId { get; set; }

        public long ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal Subtotal { get; set; }
    }
}
