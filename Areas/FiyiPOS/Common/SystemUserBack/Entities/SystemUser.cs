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

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Entities
{
    public partial class SystemUser
    {
        [Key]
        public long SystemUserId { get; set; }

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 100, pattern: "")]
        public string Name { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 50, pattern: "")]
        public string Username { get; set; } = string.Empty;

        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 255, pattern: "")]
        public string PasswordHash { get; set; } = string.Empty;

        // 'admin' | 'cashier' | 'supervisor'
        [String(nameToShowInUI: "", required: true, minimumLength: 1, maximumLength: 20, pattern: "")]
        public string Role { get; set; } = "cashier";

        public bool Active { get; set; }

        [DateTimeRange(nameToShowInUI: "", required: true, minimumDate: "0001-01-01T00:00:00.0000000", maximumDate: "9999-12-31T23:59:59.9999999")]
        public DateTime CreatedAt { get; set; }

        public string? ActivationToken { get; set; }

        public string?   PasswordResetToken           { get; set; }
        public DateTime? PasswordResetTokenExpiresAt   { get; set; }
    }
}
