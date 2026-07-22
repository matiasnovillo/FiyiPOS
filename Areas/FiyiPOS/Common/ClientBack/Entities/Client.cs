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

namespace FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Entities
{
    public partial class Client
    {
        [Key]
        public long ClientId { get; set; }

        public long StoreId { get; set; }

        [DateTimeRange(nameToShowInUI: "", required: true, minimumDate: "0001-01-01T00:00:00.0000000", maximumDate: "9999-12-31T23:59:59.9999999")]
        public DateTime DateTimeCreation { get; set; }

        [DateTimeRange(nameToShowInUI: "", required: true, minimumDate: "0001-01-01T00:00:00.0000000", maximumDate: "9999-12-31T23:59:59.9999999")]
        public DateTime DateTimeLastModification { get; set; }

        [Range(1, long.MaxValue)]
        public long UserCreationId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserLastModificationId { get; set; }

        public bool Active { get; set; }

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 200, pattern: "")]
        public string Nombres { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 200, pattern: "")]
        public string Apellidos { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 50, pattern: "")]
        public string Telefono { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 400, pattern: "")]
        public string Correo { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 500, pattern: "")]
        public string Domicilio { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 30, pattern: "")]
        public string CUIT { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: false, minimumLength: 0, maximumLength: 8000, pattern: "")]
        public string Notas { get; set; } = string.Empty;

        public bool TieneCreditoAutorizado { get; set; }

        public decimal LimiteCredito { get; set; }

        public decimal SaldoCredito { get; set; }
    }
}
