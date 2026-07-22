using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Repositories;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Repositories;

namespace FiyiPOS.Services.AFIP
{
    public class AfipTokenService(
        AfipAccessTicketRepository afipAccessTicketRepository,
        AfipWsaaClient wsaaClient,
        AfipCertificateProtectionService certificateProtection,
        AfipLogRepository afipLogRepository,
        ILogger<AfipTokenService> logger)
    {
        public async Task<(string Token, string Sign)> GetValidCredentialsAsync(
            CompanySettings config, CancellationToken ct = default)
        {
            var cached = await afipAccessTicketRepository.GetValidTicketAsync(config.Id, AfipConstants.ServiceWsfe);
            if (cached is not null)
                return (cached.Token, cached.Sign);

            logger.LogInformation("Renovando Ticket de Acceso AFIP para store {StoreId}", config.StoreId);
            return await RenewTicketAsync(config, ct);
        }

        public async Task<(string Token, string Sign)> RenewTicketAsync(
            CompanySettings config, CancellationToken ct = default)
        {
            if (config.CertificatePfx is null || string.IsNullOrEmpty(config.CertificatePassword))
                throw new InvalidOperationException("No hay certificado AFIP configurado.");

            var password = certificateProtection.Unprotect(config.CertificatePassword);
            try
            {
                var (token, sign, expiration) = await wsaaClient.LoginAsync(
                    config.CertificatePfx, password, config.Environment, AfipConstants.ServiceWsfe, ct);

                await afipAccessTicketRepository.SaveTicketAsync(new AfipAccessTicket
                {
                    CompanySettingsId = config.Id,
                    ServiceName = AfipConstants.ServiceWsfe,
                    Token = token,
                    Sign = sign,
                    ExpirationTime = expiration
                });

                await afipLogRepository.AddAsync(new AfipLog
                {
                    StoreId = config.StoreId,
                    Operation = "WSAA_Renew",
                    Response = $"TA renovado. Expira: {expiration:yyyy-MM-dd HH:mm}",
                    IsError = false
                });

                return (token, sign);
            }
            catch (Exception ex)
            {
                await afipLogRepository.AddAsync(new AfipLog
                {
                    StoreId = config.StoreId,
                    Operation = "WSAA_Renew",
                    IsError = true,
                    ErrorMessage = ex.Message
                });
                throw;
            }
        }
    }
}
