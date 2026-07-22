using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.DatabaseContexts;
using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Interfaces;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Repositories
{
    public partial class StoreRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IStoreRepository
    {
        public async Task<int> CountAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Store.AsNoTracking().CountAsync();
        }

        public async Task<Store?> GetOneByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Store
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StoreId == storeId);
        }

        public async Task<List<Store>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Store
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Store>> GetAllActiveAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Store
                .AsNoTracking()
                .Where(x => x.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Store store)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            EntityEntry<Store> StoreToAdd = await DbContext.Store.AddAsync(store);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Store store)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            DbContext.Store.Update(store);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteOneByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Store
                .Where(x => x.StoreId == storeId)
                .ExecuteDeleteAsync();
        }

        public async Task ToggleActiveAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            bool current = await DbContext.Store
                .Where(x => x.StoreId == storeId)
                .Select(x => x.Active)
                .FirstOrDefaultAsync();
            await DbContext.Store
                .Where(x => x.StoreId == storeId)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.Active, !current));
        }
    }
}
