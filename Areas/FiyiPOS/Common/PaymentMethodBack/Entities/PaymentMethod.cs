using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Entities
{
    public partial class PaymentMethod
    {
        [Key]
        public long PaymentMethodId { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool RequiresRef { get; set; }

        public bool IsCash { get; set; }

        public bool Active { get; set; }
    }
}
