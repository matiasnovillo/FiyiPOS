using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.DTOs;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Interfaces
{
    public partial interface ISupplierRepository
    {
        #region Async Queries
        Task<int> CountAsync();

        Task<Supplier?> GetOneBySupplierIdAsync(long supplierId);

        Task<List<Supplier>> GetAllAsync();

        Task<List<Supplier>> GetAllActiveAsync(long storeId = 0);

        Task<List<Supplier>> GetAllBySupplierIdCheckedAsync(List<long> listCheckedSupplierIds);

        Task<List<Supplier>> GetAllForModalAsync(string textToSearch);

        Task<PaginatedSupplierDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize);

        Task<PaginatedSupplierDTO> GetAllPaginatedBySearchAsync(string textToSearch,
            int pageIndex,
            int pageSize,
            bool? activeFilter = null,
            long storeId = 0);

        Task<int> CountActiveAsync(long storeId = 0);
        #endregion

        #region Async Non-Queries
        Task<int> AddAsync(Supplier supplier);

        Task<int> AddRangeAsync(List<Supplier> listSupplier);

        Task<int> UpdateAsync(Supplier supplier);

        Task<int> DeleteOneBySupplierIdAsync(long supplierId);

        Task<int> DeleteAllAsync();

        Task<int> DeleteManyAsync(List<Supplier> listSupplier);
        #endregion

        #region Methods for DataTable
        Task<DataTable> GetAllInDataTableAsync();

        Task<DataTable> GetAllBySupplierIdInDataTableAsync(List<long> listCheckedSupplierIds);
        #endregion
    }
}
