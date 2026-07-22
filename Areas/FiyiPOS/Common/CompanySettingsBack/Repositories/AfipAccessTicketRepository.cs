using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Interfaces;
using FiyiPOS.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Repositories
{
    public class AfipAccessTicketRepository(IDbContextFactory<FiyiPOSDbContext> factory) : IAfipAccessTicketRepository
    {
        public async Task<AfipAccessTicket?> GetValidTicketAsync(int companySettingsId, string serviceName)
        {
            await using var db = await factory.CreateDbContextAsync();
            var margin = DateTime.Now.AddMinutes(5);
            return await db.AfipAccessTicket.AsNoTracking()
                .Where(t => t.CompanySettingsId == companySettingsId
                         && t.ServiceName == serviceName
                         && t.ExpirationTime > margin)
                .OrderByDescending(t => t.ExpirationTime)
                .FirstOrDefaultAsync();
        }

        public async Task SaveTicketAsync(AfipAccessTicket ticket)
        {
            await using var db = await factory.CreateDbContextAsync();
            var existing = await db.AfipAccessTicket
                .FirstOrDefaultAsync(t => t.CompanySettingsId == ticket.CompanySettingsId
                                       && t.ServiceName == ticket.ServiceName);

            if (existing is null)
            {
                ticket.CreatedAt = DateTime.Now;
                await db.AfipAccessTicket.AddAsync(ticket);
            }
            else
            {
                existing.Token = ticket.Token;
                existing.Sign = ticket.Sign;
                existing.ExpirationTime = ticket.ExpirationTime;
                existing.CreatedAt = DateTime.Now;
            }

            await db.SaveChangesAsync();
        }
    }
}
