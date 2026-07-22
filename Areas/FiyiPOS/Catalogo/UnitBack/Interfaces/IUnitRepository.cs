using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.DTOs;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Interfaces
{
    public partial interface IUnitRepository
    {
        #region Async Queries
        Task<int> CountAsync();

        Task<Unit?> GetOneByUnitIdAsync(long unitId);

        Task<List<Unit>> GetAllAsync();

        Task<List<Unit>> GetAllByStoreIdAsync(long storeId);

        Task<List<Unit>> GetAllActiveAsync(long storeId);

        Task<PaginatedUnitDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize);
        #endregion

        #region Async Non-Queries
        Task<int> AddAsync(Unit unit);

        Task AddRangeAsync(IEnumerable<Unit> units);

        Task<int> UpdateAsync(Unit unit);

        Task<int> DeleteOneByUnitIdAsync(long unitId);

        Task<int> DeleteAllAsync();
        #endregion

        #region Methods for DataTable
        Task<DataTable> GetAllInDataTableAsync();
        #endregion
    }
}
