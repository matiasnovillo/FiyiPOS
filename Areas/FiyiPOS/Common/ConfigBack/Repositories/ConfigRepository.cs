using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Repositories
{
    public class ConfigRepository(IDbContextFactory<FiyiPOSDbContext> _factory) : IConfigRepository
    {
        public async Task<List<Config>> GetAllGlobalAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Config
                .AsNoTracking()
                .Where(c => c.StoreId == null)
                .OrderBy(c => c.ConfigId)
                .ToListAsync();
        }

        public async Task UpdateValueAsync(int configId, string value)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await db.Config
                .Where(c => c.ConfigId == configId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.Value, value));
        }

        public async Task<List<Config>> GetAllByStoreIdAsync(long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Config
                .AsNoTracking()
                .Where(c => c.StoreId == storeId)
                .OrderBy(c => c.ConfigId)
                .ToListAsync();
        }

        public async Task<string?> GetValueByStoreAsync(long storeId, string key)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Config
                .AsNoTracking()
                .Where(c => c.StoreId == storeId && c.ConfigKey == key)
                .Select(c => c.Value)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetValueAsync(string key)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Config
                .AsNoTracking()
                .Where(c => c.ConfigKey == key && c.StoreId == null)
                .Select(c => c.Value)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Config>> GetSchemeVarsAsync(long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Config
                .AsNoTracking()
                .Where(c => c.StoreId == storeId && c.ConfigKey.StartsWith("pos_"))
                .OrderBy(c => c.ConfigId)
                .ToListAsync();
        }

        public async Task UpdateSchemeVarsAsync(long storeId, Dictionary<string, string> updates)
        {
            await using var db = await _factory.CreateDbContextAsync();
            foreach (var (key, value) in updates)
            {
                await db.Config
                    .Where(c => c.StoreId == storeId && c.ConfigKey == key)
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.Value, value));
            }
        }

        public async Task AddRangeAsync(IEnumerable<Config> configs)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await db.Config.AddRangeAsync(configs);
            await db.SaveChangesAsync();
        }
    }
}
