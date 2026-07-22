using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Entities
{
    public partial class SaleStatus
    {
        [Key]
        public int SaleStatusId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
