using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Interfaces;
using FiyiPOS.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Repositories
{
    public class AfipLogRepository(IDbContextFactory<FiyiPOSDbContext> factory) : IAfipLogRepository
    {
        public async Task AddAsync(AfipLog log)
        {
            await using var db = await factory.CreateDbContextAsync();
            log.CreatedAt = DateTime.Now;
            await db.AfipLog.AddAsync(log);
            await db.SaveChangesAsync();
        }
    }
}
