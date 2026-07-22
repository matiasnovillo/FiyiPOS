using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Entities
{
    public partial class Purchase
    {
        [Key]
        public long PurchaseId { get; set; }

        public long StoreId { get; set; }

        public long SupplierId { get; set; }

        public long SystemUserId { get; set; }

        public string? SupplierInvoice { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal Subtotal { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal Total { get; set; }

        public string? Notes { get; set; }

        // false = pendiente (stock no tocado), true = confirmada (stock actualizado)
        public bool IsConfirmed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
