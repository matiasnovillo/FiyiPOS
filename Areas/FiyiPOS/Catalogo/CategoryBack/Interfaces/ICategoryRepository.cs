using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.DTOs;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Interfaces
{
    public partial interface ICategoryRepository
    {
        #region Async Queries
        Task<int> CountAsync();

        Task<Category?> GetOneByCategoryIdAsync(long categoryId);

        Task<List<Category>> GetAllAsync();

        Task<List<Category>> GetAllByStoreIdAsync(long storeId);

        Task<List<Category>> GetAllActiveAsync(long storeId);

        Task<PaginatedCategoryDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize);
        #endregion

        #region Async Non-Queries
        Task<int> AddAsync(Category category);

        Task<int> UpdateAsync(Category category);

        Task<int> DeleteOneByCategoryIdAsync(long categoryId);

        Task<int> DeleteAllAsync();
        #endregion

        #region Methods for DataTable
        Task<DataTable> GetAllInDataTableAsync();
        #endregion
    }
}
