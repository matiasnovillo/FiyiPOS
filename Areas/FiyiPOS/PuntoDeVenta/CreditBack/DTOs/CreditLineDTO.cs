namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.DTOs
{
    public class CreditLineDTO
    {
        public long      CreditId       { get; set; }
        public long      ClientId       { get; set; }
        public string    ClientName     { get; set; } = "";
        public string    ClientPhone    { get; set; } = "";
        public decimal   OriginalAmount { get; set; }
        public decimal   Balance        { get; set; }
        public decimal   AmountPaid     => OriginalAmount - Balance;
        public DateTime? DueDate        { get; set; }
        public bool      IsOverdue      => DueDate.HasValue && DueDate.Value.Date < DateTime.Today && Balance > 0;
        public bool      Active         { get; set; }
        public DateTime  CreatedAt      { get; set; }
        public string?   Notes          { get; set; }
    }
}
