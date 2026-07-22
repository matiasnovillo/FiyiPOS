using FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Interfaces
{
    public interface IConfigRepository
    {
        Task<List<Config>> GetAllGlobalAsync();
        Task<List<Config>> GetAllByStoreIdAsync(long storeId);
        Task UpdateValueAsync(int configId, string value);
        Task<string?> GetValueAsync(string key);
        Task<string?> GetValueByStoreAsync(long storeId, string key);
        Task<List<Config>> GetSchemeVarsAsync(long storeId);
        Task UpdateSchemeVarsAsync(long storeId, Dictionary<string, string> updates);
        Task AddRangeAsync(IEnumerable<Config> configs);
    }
}
