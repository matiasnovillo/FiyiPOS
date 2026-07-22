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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Entities
{
    public partial class Unit
    {
        [Key]
        public long UnitId { get; set; }

        public long StoreId { get; set; }

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 50, pattern: "")]
        public string Name { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 10, pattern: "")]
        public string Abbreviation { get; set; } = string.Empty;

        public bool Active { get; set; }
    }
}
