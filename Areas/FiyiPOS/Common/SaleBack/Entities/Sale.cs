using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Entities
{
    public partial class Sale
    {
        [Key]
        public long SaleId { get; set; }

        public long StoreId { get; set; }

        public long SystemUserId { get; set; }

        public long? ClientId { get; set; }

        public int SaleStatusId { get; set; }

        public long? PriceListId { get; set; }

        public string TicketNumber { get; set; } = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Discount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal Total { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal Change { get; set; }

        public string? Notes { get; set; }

        public bool IsPrinted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
