using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Interfaces;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Repositories
{
    public partial class CategoryRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ICategoryRepository
    {
        #region Async Queries
        public async Task<int> CountAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Category.AsNoTracking().CountAsync();
        }

        public async Task<Category?> GetOneByCategoryIdAsync(long categoryId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Category
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Category
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Category
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllActiveAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Category
                .AsNoTracking()
                .Where(x => x.Active && x.StoreId == storeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<PaginatedCategoryDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            string[] Words = Regex
                .Replace(textToSearch.Trim(), @"\s+", " ")
                .Split(" ");

            List<Category> ListCategory = await DbContext.Category
                .AsNoTracking()
                .Where(x => strictSearch
                    ? Words.All(word => x.Name.Contains(word) || x.Description.Contains(word))
                    : Words.Any(word => x.Name.Contains(word) || x.Description.Contains(word)))
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            long TotalRecordsInDatabase = await DbContext.Category.LongCountAsync();

            return new PaginatedCategoryDTO
            {
                ListCategory            = ListCategory,
                TotalRecordsInDatabase  = TotalRecordsInDatabase,
                TotalRecordsInTheList   = ListCategory.Count,
                PageIndex               = pageIndex,
                PageSize                = pageSize
            };
        }
        #endregion

        #region Async Non-Queries
        public async Task<int> AddAsync(Category category)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            EntityEntry<Category> CategoryToAdd = await DbContext.Category.AddAsync(category);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Category category)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            DbContext.Category.Update(category);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteOneByCategoryIdAsync(long categoryId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Category
                .Where(x => x.CategoryId == categoryId)
                .ExecuteDeleteAsync();
        }

        public async Task<int> DeleteAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Category.ExecuteDeleteAsync();
        }
        #endregion

        #region Methods for DataTable
        public async Task<DataTable> GetAllInDataTableAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Category> ListCategory = await DbContext.Category
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("CategoryId",   typeof(string));
            DataTable.Columns.Add("Name",         typeof(string));
            DataTable.Columns.Add("Description",  typeof(string));
            DataTable.Columns.Add("Active",       typeof(string));

            foreach (Category Category in ListCategory)
            {
                DataTable.Rows.Add(
                    Category.CategoryId,
                    Category.Name,
                    Category.Description,
                    Category.Active
                );
            }

            return DataTable;
        }
        #endregion
    }
}
