using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.Entities;
using FiyiPOS.DatabaseContexts;
using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.Interfaces;

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.Repositories
{
    public class SystemUserStoreRepository(IDbContextFactory<FiyiPOSDbContext> _factory)
        : ISystemUserStoreRepository
    {
        public async Task<List<Store>> GetStoresByUserIdAsync(long systemUserId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.SystemUserStore
                .AsNoTracking()
                .Where(x => x.SystemUserId == systemUserId)
                .Select(x => x.Store!)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task AssignAsync(long systemUserId, long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            bool exists = await db.SystemUserStore
                .AnyAsync(x => x.SystemUserId == systemUserId && x.StoreId == storeId);
            if (exists) return;
            await db.SystemUserStore.AddAsync(new SystemUserStore
            {
                SystemUserId = systemUserId,
                StoreId      = storeId
            });
            await db.SaveChangesAsync();
        }

        public async Task RemoveAsync(long systemUserId, long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await db.SystemUserStore
                .Where(x => x.SystemUserId == systemUserId && x.StoreId == storeId)
                .ExecuteDeleteAsync();
        }
    }
}
