namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.DTOs
{
    public class InventoryMovementLineDTO
    {
        public long     InventoryMovementId { get; set; }
        public string   ProductName         { get; set; } = "";
        public string   Barcode             { get; set; } = "";
        public string   MovementType        { get; set; } = "";
        public decimal  Quantity            { get; set; }
        public decimal  StockBefore         { get; set; }
        public decimal  StockAfter          { get; set; }
        public string?  Reason              { get; set; }
        public string?  Reference           { get; set; }
        public DateTime MovementDate        { get; set; }
    }
}
