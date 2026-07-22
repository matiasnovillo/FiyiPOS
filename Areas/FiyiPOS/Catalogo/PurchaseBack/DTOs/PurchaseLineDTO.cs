namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.DTOs
{
    public record PurchaseLineDTO
    {
        public long     PurchaseId      { get; init; }
        public string   SupplierName    { get; init; } = "";
        public string?  SupplierInvoice { get; init; }
        public DateTime PurchaseDate    { get; init; }
        public decimal  Total           { get; init; }
        public bool     IsConfirmed     { get; init; }
        public string?  Notes           { get; init; }
        public long     SupplierId      { get; init; }
        public long     SystemUserId    { get; init; }
        public decimal  Subtotal        { get; init; }
        public decimal  TaxAmount       { get; init; }
    }
}
