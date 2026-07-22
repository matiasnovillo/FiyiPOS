using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Interfaces
{
    public interface ICompanySettingsRepository
    {
        Task<CompanySettings?> GetByStoreIdAsync(long storeId);
        Task<CompanySettings?> GetByStoreIdForUpdateAsync(long storeId);
        Task<CompanySettings> UpsertAsync(CompanySettings config);
    }
}
