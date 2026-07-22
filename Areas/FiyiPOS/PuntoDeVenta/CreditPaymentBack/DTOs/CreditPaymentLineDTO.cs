namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.DTOs
{
    public class CreditPaymentLineDTO
    {
        public long     CreditPaymentId     { get; set; }
        public string   PaymentMethodName   { get; set; } = "";
        public decimal  Amount              { get; set; }
        public DateTime PaymentDate         { get; set; }
        public string?  Notes               { get; set; }
    }
}
