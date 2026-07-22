namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Entities
{
    public class CreditPayment
    {
        public long     CreditPaymentId { get; set; }
        public long     CreditId        { get; set; }
        public long     SystemUserId    { get; set; }
        public long     PaymentMethodId { get; set; }
        public decimal  Amount          { get; set; }
        public DateTime PaymentDate     { get; set; }
        public string?  Notes           { get; set; }
    }
}
