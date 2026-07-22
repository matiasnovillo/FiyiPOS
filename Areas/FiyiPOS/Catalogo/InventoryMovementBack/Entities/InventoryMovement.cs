using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities
{
    public partial class InventoryMovement
    {
        [Key]
        public long InventoryMovementId { get; set; }

        public long StoreId { get; set; }

        public long ProductId { get; set; }

        public long SystemUserId { get; set; }

        public long? SupplierId { get; set; }

        public long? SaleId { get; set; }

        public long? PurchaseId { get; set; }

        // 'IN' | 'OUT' | 'ADJUST'
        public string MovementType { get; set; } = string.Empty;

        // Positive = entry, negative = exit, delta = adjust
        public decimal Quantity { get; set; }

        public decimal StockBefore { get; set; }

        public decimal StockAfter { get; set; }

        public decimal? UnitCost { get; set; }

        public string? Reference { get; set; }

        public string? Reason { get; set; }

        public DateTime MovementDate { get; set; }
    }
}
