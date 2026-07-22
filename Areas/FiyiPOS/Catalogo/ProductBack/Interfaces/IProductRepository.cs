using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.DTOs;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Interfaces
{
    public partial interface IProductRepository
    {
        #region Async Queries
        Task<int> CountAsync();

        Task<Product?> GetOneByProductIdAsync(long productId);

        Task<Product?> GetOneByBarcodeAsync(string barcode);

        Task<List<Product>> GetAllAsync();

        Task<List<Product>> GetAllByStoreIdAsync(long storeId);

        Task<List<Product>> GetAllActiveByStoreIdAsync(long storeId);

        Task<List<Product>> GetAllActiveAsync();

        Task<PaginatedProductDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize);

        Task<PaginatedProductDTO> GetAllPaginatedByStoreIdAsync(long storeId,
            string textToSearch,
            int pageIndex,
            int pageSize,
            bool onlyLowStock = false,
            bool? activeFilter = null,
            bool onlyNoBarcode = false,
            bool onlyNoStock = false,
            bool onlyOnline = false);

        Task<int> CountActiveByStoreIdAsync(long storeId);
        Task<int> CountLowStockByStoreIdAsync(long storeId);
        Task<int> CountNoBarcodeByStoreIdAsync(long storeId);
        Task<int> CountNoStockByStoreIdAsync(long storeId);
        Task<int> CountOnlineByStoreIdAsync(long storeId);
        Task<List<Product>> GetManyByProductIdsAsync(IEnumerable<long> ids);
        #endregion

        #region Async Non-Queries
        Task<int> AddAsync(Product product);

        Task<int> UpdateAsync(Product product);

        Task<int> DeleteOneByProductIdAsync(long productId);

        Task<int> DeleteAllAsync();
        #endregion

        #region Methods for DataTable
        Task<DataTable> GetAllInDataTableAsync();

        Task<DataTable> GetAllInDataTableByStoreIdAsync(long storeId);
        #endregion
    }
}
