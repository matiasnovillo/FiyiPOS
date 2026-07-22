using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Interfaces
{
    public partial interface IStoreRepository
    {
        Task<int> CountAsync();
        Task<Store?> GetOneByStoreIdAsync(long storeId);
        Task<List<Store>> GetAllAsync();
        Task<List<Store>> GetAllActiveAsync();
        Task<int> AddAsync(Store store);
        Task<int> UpdateAsync(Store store);
        Task<int> DeleteOneByStoreIdAsync(long storeId);
        Task ToggleActiveAsync(long storeId);
    }
}
