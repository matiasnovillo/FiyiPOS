using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.DTOs;
using System.Data;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Interfaces
{
    public partial interface IClientRepository
    {
        #region Async Queries
        Task<int> CountAsync();

        Task<Client?> GetOneByClientIdAsync(long clientId);

        Task<List<Client>> GetAllAsync();

        Task<List<Client>> GetAllByStoreIdAsync(long storeId);

        Task<List<Client>> GetAllByClientIdCheckedAsync(List<long> listCheckedClientIds);

        Task<List<Client>> GetAllForModalAsync(string textToSearch);

        Task<PaginatedClientDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize);

        Task<PaginatedClientDTO> GetAllPaginatedByStoreIdAsync(
            long storeId,
            string textToSearch,
            int pageIndex,
            int pageSize,
            List<long>? clientIdFilter = null);

        Task<int> CountWithCreditByStoreIdAsync(long storeId);
        #endregion

        #region Async Non-Queries
        Task<int> AddAsync(Client client);

        Task<int> AddRangeAsync(List<Client> listClient);

        Task<int> UpdateAsync(Client client);

        Task<int> DeleteOneByClientIdAsync(long clientId);

        Task<int> DeleteAllAsync();

        Task<int> DeleteManyAsync(List<Client> listClient);
        #endregion

        #region Methods for DataTable
        Task<DataTable> GetAllByClientIdInDataTableAsync(List<long> listCheckedClientIds);

        Task<DataTable> GetAllInDataTableAsync();

        Task<DataTable> GetAllInDataTableByStoreIdAsync(long storeId);
        #endregion
    }
}
