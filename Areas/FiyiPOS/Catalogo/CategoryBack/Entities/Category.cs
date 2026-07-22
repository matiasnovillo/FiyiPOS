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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Entities
{
    public partial class Category
    {
        [Key]
        public long CategoryId { get; set; }

        public long StoreId { get; set; }

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 100, pattern: "")]
        public string Name { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 300, pattern: "")]
        public string Description { get; set; } = string.Empty;

        public bool Active { get; set; }
    }
}
