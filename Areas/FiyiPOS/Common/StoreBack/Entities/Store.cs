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

namespace FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities
{
    public partial class Store
    {
        [Key]
        public long StoreId { get; set; }

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 150, pattern: "")]
        public string Name { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 300, pattern: "")]
        public string Address { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 30, pattern: "")]
        public string Phone { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 30, pattern: "")]
        public string TaxId { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 500, pattern: "")]
        public string LogoUrl { get; set; } = string.Empty;

        public bool Active { get; set; }

        [DateTimeRange(nameToShowInUI: "", required: true, minimumDate: "0001-01-01T00:00:00.0000000", maximumDate: "9999-12-31T23:59:59.9999999")]
        public DateTime CreatedAt { get; set; }

        public DateTime? BillingDueDate { get; set; }

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 500, pattern: "")]
        public string? ClaudeApiKey { get; set; }
    }
}
