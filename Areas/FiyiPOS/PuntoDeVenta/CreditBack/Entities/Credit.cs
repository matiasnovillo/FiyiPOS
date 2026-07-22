namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Entities
{
    public class Credit
    {
        public long      CreditId       { get; set; }
        public long      StoreId        { get; set; }
        public long      ClientId       { get; set; }
        public long?     SaleId         { get; set; }   // null when created manually
        public long      SystemUserId   { get; set; }
        public decimal   OriginalAmount { get; set; }
        public decimal   Balance        { get; set; }   // remaining debt
        public DateTime? DueDate        { get; set; }
        public string?   Notes          { get; set; }
        public bool      Active         { get; set; } = true;  // false when fully paid
        public DateTime  CreatedAt      { get; set; }
    }
}
