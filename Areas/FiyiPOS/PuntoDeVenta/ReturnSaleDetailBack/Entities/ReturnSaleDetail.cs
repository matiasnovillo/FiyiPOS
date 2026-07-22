using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleDetailBack.Entities
{
    public partial class ReturnSaleDetail
    {
        [Key]
        public long ReturnSaleDetailId { get; set; }

        public long ReturnSaleId { get; set; }

        public long ProductId { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }
    }
}
