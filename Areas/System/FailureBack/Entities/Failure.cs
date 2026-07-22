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

namespace FiyiPOS.Areas.System.FailureBack.Entities
{
    public partial class Failure
    {
        [Key]
        public long FailureId { get; set; }

        [DateTimeRange(nameToShowInUI: "", required: true, minimumDate: "0001-01-01T00:00:00.0000000", maximumDate: "9999-12-31T23:59:59.9999999")]
        public DateTime DateTimeCreation { get; set; }

        [DateTimeRange(nameToShowInUI: "", required: true, minimumDate: "0001-01-01T00:00:00.0000000", maximumDate: "9999-12-31T23:59:59.9999999")]
        public DateTime DateTimeLastModification { get; set; }

        [Range(1, long.MaxValue)]
        public long UserCreationId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserLastModificationId { get; set; }

        public bool Active { get; set; }

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 8000, pattern: "")]
        public string Message { get; set; } = string.Empty;

        [Long(nameToShowInUI: "", required: true, minimumLong: 0, maximumLong: 2147483647)]        
        public long EmergencyLevel { get; set; }

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 8000, pattern: "")]
        public string StackTrace { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 8000, pattern: "")]
        public string Source { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 8000, pattern: "")]
        public string Comment { get; set; } = string.Empty;
    }
}