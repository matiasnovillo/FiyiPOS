using FiyiPOS.Library.ModelAttributeValidator;
using System.ComponentModel.DataAnnotations;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Entities
{
    public partial class Product
    {
        [Key]
        public long ProductId { get; set; }

        public long StoreId { get; set; }

        public long? CategoryId { get; set; }

        public long? UnitId { get; set; }

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 50, pattern: "")]
        public string Barcode { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 150, pattern: "")]
        public string Name { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 8000, pattern: "")]
        public string Description { get; set; } = string.Empty;

        public decimal SalePrice { get; set; }

        public decimal NightSalePrice { get; set; }

        public decimal CostPrice { get; set; }

        public decimal? WholesalePrice { get; set; }

        public decimal Stock { get; set; }

        public decimal MinStock { get; set; }

        public decimal? MaxStock { get; set; }

        public decimal TaxRate { get; set; }

        public bool AllowFractional { get; set; }

        public bool Active { get; set; }

        public bool IsAvailableToSellOnline { get; set; }

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 500, pattern: "")]
        public string FirstImageURL { get; set; } = string.Empty;

        [DateTimeRange(nameToShowInUI: "", required: true, minimumDate: "0001-01-01T00:00:00.0000000", maximumDate: "9999-12-31T23:59:59.9999999")]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
