using Microsoft.AspNetCore.DataProtection;

namespace FiyiPOS.Services.AFIP
{
    public class AfipCertificateProtectionService(IDataProtectionProvider dataProtectionProvider)
    {
        private readonly IDataProtector _protector =
            dataProtectionProvider.CreateProtector("FiyiPOS.Afip.CertificatePassword");

        public string Protect(string plainPassword) => _protector.Protect(plainPassword);

        public string Unprotect(string protectedPassword) => _protector.Unprotect(protectedPassword);
    }
}
