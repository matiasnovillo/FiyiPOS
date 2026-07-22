using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Entities
{
    public partial class SalePayment
    {
        [Key]
        public long SalePaymentId { get; set; }

        public long SaleId { get; set; }

        public long PaymentMethodId { get; set; }

        public decimal Amount { get; set; }

        public string? Reference { get; set; }
    }
}
