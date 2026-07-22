using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Entities
{
    public partial class ReturnSale
    {
        [Key]
        public long ReturnSaleId { get; set; }

        public long SaleId { get; set; }

        public long StoreId { get; set; }

        public long SystemUserId { get; set; }

        public DateTime ReturnDate { get; set; }

        public string? Reason { get; set; }

        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
