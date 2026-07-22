namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.DTOs
{
    public record PurchaseDetailLineDTO
    {
        public long    PurchaseDetailId { get; init; }
        public long    ProductId        { get; init; }
        public string  ProductName      { get; init; } = "";
        public string  Barcode          { get; init; } = "";
        public decimal Quantity         { get; init; }
        public decimal UnitCost         { get; init; }
        public decimal Subtotal         { get; init; }
    }
}
