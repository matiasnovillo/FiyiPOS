using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Interfaces;
using FiyiPOS.DatabaseContexts;
using System.Text.RegularExpressions;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Repositories
{
    public partial class ProductRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IProductRepository
    {
        #region Async Queries
        public async Task<int> CountAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product.AsNoTracking().CountAsync();
        }

        public async Task<Product?> GetOneByProductIdAsync(long productId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<Product?> GetOneByBarcodeAsync(string barcode)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Barcode == barcode && x.Active);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllActiveByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product
                .AsNoTracking()
                .Where(x => x.StoreId == storeId && x.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllActiveAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product
                .AsNoTracking()
                .Where(x => x.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<PaginatedProductDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            string[] Words = Regex
                .Replace(textToSearch.Trim(), @"\s+", " ")
                .Split(" ");

            List<Product> ListProduct = await DbContext.Product
                .AsNoTracking()
                .Where(x => strictSearch
                    ? Words.All(word =>
                        x.Name.Contains(word) ||
                        x.Barcode.Contains(word) ||
                        x.Description.Contains(word))
                    : Words.Any(word =>
                        x.Name.Contains(word) ||
                        x.Barcode.Contains(word) ||
                        x.Description.Contains(word)))
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            long TotalRecordsInDatabase = await DbContext.Product.LongCountAsync();

            return new PaginatedProductDTO
            {
                ListProduct             = ListProduct,
                TotalRecordsInDatabase  = TotalRecordsInDatabase,
                TotalRecordsInTheList   = ListProduct.Count,
                TotalFilteredRecords    = ListProduct.Count,
                PageIndex               = pageIndex,
                PageSize                = pageSize
            };
        }

        public async Task<PaginatedProductDTO> GetAllPaginatedByStoreIdAsync(
            long storeId,
            string textToSearch,
            int pageIndex,
            int pageSize,
            bool onlyLowStock = false,
            bool? activeFilter = null,
            bool onlyNoBarcode = false,
            bool onlyNoStock = false,
            bool onlyOnline = false)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            IQueryable<Product> query = DbContext.Product
                .AsNoTracking()
                .Where(x => x.StoreId == storeId);

            if (!string.IsNullOrWhiteSpace(textToSearch))
            {
                string[] Words = Regex
                    .Replace(textToSearch
                    .Trim(), @"\s+", " ")
                    .Split(" ");

                query = query.Where(x =>
                    Words.Any(word => x.Name.Contains(word)) ||
                    Words.Any(word => x.Barcode.Contains(word)) ||
                    Words.Any(word => x.Description.Contains(word)));
            }

            if (onlyLowStock)
                query = query.Where(x => x.Stock <= x.MinStock);

            if (activeFilter.HasValue)
                query = query.Where(x => x.Active == activeFilter.Value);

            if (onlyNoBarcode)
                query = query.Where(x => x.Barcode == "");

            if (onlyNoStock)
                query = query.Where(x => x.Stock == x.MinStock);

            if (onlyOnline)
                query = query.Where(x => x.IsAvailableToSellOnline);

            long totalFiltered = await query.LongCountAsync();
            long totalInDb     = await DbContext.Product.LongCountAsync(x => x.StoreId == storeId);

            List<Product> list = await query
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedProductDTO
            {
                ListProduct            = list,
                TotalRecordsInDatabase = totalInDb,
                TotalRecordsInTheList  = list.Count,
                TotalFilteredRecords   = totalFiltered,
                PageIndex              = pageIndex,
                PageSize               = pageSize
            };
        }

        public async Task<int> CountActiveByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Product
                .AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.Active);
        }

        public async Task<int> CountLowStockByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Product
                .AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.Active && x.Stock <= x.MinStock);
        }

        public async Task<int> CountNoBarcodeByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Product
                .AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.Barcode == "");
        }

        public async Task<int> CountNoStockByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Product
                .AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.Stock == x.MinStock);
        }

        public async Task<int> CountOnlineByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Product
                .AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.IsAvailableToSellOnline);
        }

        public async Task<List<Product>> GetManyByProductIdsAsync(IEnumerable<long> ids)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            var idList = ids.ToList();
            return await DbContext.Product
                .AsNoTracking()
                .Where(x => idList.Contains(x.ProductId))
                .ToListAsync();
        }
        #endregion

        #region Async Non-Queries
        public async Task<int> AddAsync(Product product)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            SanitizeStocks(product);
            EntityEntry<Product> ProductToAdd = await DbContext.Product.AddAsync(product);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Product product)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            SanitizeStocks(product);
            DbContext.Product.Update(product);

            return await DbContext.SaveChangesAsync();
        }

        // Rejects decimal stock values when the product does not allow fractional quantities.
        private static void SanitizeStocks(Product p)
        {
            if (p.AllowFractional) return;
            if (p.Stock    != Math.Floor(p.Stock)    ||
                p.MinStock != Math.Floor(p.MinStock) ||
                (p.MaxStock.HasValue && p.MaxStock.Value != Math.Floor(p.MaxStock.Value)))
            {
                throw new InvalidOperationException(
                    "Este producto no permite venta fraccionada. Los valores de stock deben ser números enteros.");
            }
        }

        public async Task<int> DeleteOneByProductIdAsync(long productId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product
                .Where(x => x.ProductId == productId)
                .ExecuteDeleteAsync();
        }

        public async Task<int> DeleteAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Product.ExecuteDeleteAsync();
        }
        #endregion

        #region Methods for DataTable
        public async Task<DataTable> GetAllInDataTableByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Product> ListProduct = await DbContext.Product
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("ProductId",       typeof(string));
            DataTable.Columns.Add("Barcode",         typeof(string));
            DataTable.Columns.Add("Name",            typeof(string));
            DataTable.Columns.Add("Description",     typeof(string));
            DataTable.Columns.Add("SalePrice",       typeof(string));
            DataTable.Columns.Add("CostPrice",       typeof(string));
            DataTable.Columns.Add("WholesalePrice",  typeof(string));
            DataTable.Columns.Add("Stock",           typeof(string));
            DataTable.Columns.Add("MinStock",        typeof(string));
            DataTable.Columns.Add("MaxStock",        typeof(string));
            DataTable.Columns.Add("TaxRate",         typeof(string));
            DataTable.Columns.Add("AllowFractional", typeof(string));
            DataTable.Columns.Add("Active",          typeof(string));

            foreach (Product Product in ListProduct)
            {
                DataTable.Rows.Add(
                    Product.ProductId,
                    Product.Barcode,
                    Product.Name,
                    Product.Description,
                    Product.SalePrice,
                    Product.CostPrice,
                    Product.WholesalePrice,
                    Product.Stock,
                    Product.MinStock,
                    Product.MaxStock,
                    Product.TaxRate,
                    Product.AllowFractional,
                    Product.Active
                );
            }

            return DataTable;
        }

        public async Task<DataTable> GetAllInDataTableAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Product> ListProduct = await DbContext.Product
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("ProductId",      typeof(string));
            DataTable.Columns.Add("CategoryId",     typeof(string));
            DataTable.Columns.Add("UnitId",         typeof(string));
            DataTable.Columns.Add("Barcode",        typeof(string));
            DataTable.Columns.Add("Name",           typeof(string));
            DataTable.Columns.Add("Description",    typeof(string));
            DataTable.Columns.Add("SalePrice",      typeof(string));
            DataTable.Columns.Add("CostPrice",      typeof(string));
            DataTable.Columns.Add("WholesalePrice", typeof(string));
            DataTable.Columns.Add("Stock",          typeof(string));
            DataTable.Columns.Add("MinStock",       typeof(string));
            DataTable.Columns.Add("MaxStock",       typeof(string));
            DataTable.Columns.Add("TaxRate",        typeof(string));
            DataTable.Columns.Add("AllowFractional",typeof(string));
            DataTable.Columns.Add("Active",         typeof(string));
            DataTable.Columns.Add("CreatedAt",      typeof(string));
            DataTable.Columns.Add("UpdatedAt",      typeof(string));

            foreach (Product Product in ListProduct)
            {
                DataTable.Rows.Add(
                    Product.ProductId,
                    Product.CategoryId,
                    Product.UnitId,
                    Product.Barcode,
                    Product.Name,
                    Product.Description,
                    Product.SalePrice,
                    Product.CostPrice,
                    Product.WholesalePrice,
                    Product.Stock,
                    Product.MinStock,
                    Product.MaxStock,
                    Product.TaxRate,
                    Product.AllowFractional,
                    Product.Active,
                    Product.CreatedAt,
                    Product.UpdatedAt
                );
            }

            return DataTable;
        }
        #endregion
    }
}
