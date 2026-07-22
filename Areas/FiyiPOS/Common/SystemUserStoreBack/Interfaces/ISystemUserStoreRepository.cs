using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.Interfaces
{
    public interface ISystemUserStoreRepository
    {
        Task<List<Store>> GetStoresByUserIdAsync(long systemUserId);
        Task AssignAsync(long systemUserId, long storeId);
        Task RemoveAsync(long systemUserId, long storeId);
    }
}
