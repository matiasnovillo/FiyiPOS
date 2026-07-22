using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Interfaces;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Repositories
{
    public partial class SupplierRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ISupplierRepository
    {
        #region Async Queries
        public async Task<int> CountAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<Supplier?> GetOneBySupplierIdAsync(long supplierId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SupplierId == supplierId);
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Supplier>> GetAllActiveAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier
                .AsNoTracking()
                .Where(x => x.Active && x.StoreId == storeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Supplier>> GetAllBySupplierIdCheckedAsync(List<long> listCheckedSupplierIds)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier
                .AsNoTracking()
                .Where(x => listCheckedSupplierIds.Contains(x.SupplierId))
                .ToListAsync();
        }

        public async Task<List<Supplier>> GetAllForModalAsync(string textToSearch)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier
                .AsNoTracking()
                .Where(x => x.Active &&
                           (x.Name.Contains(textToSearch) ||
                            x.ContactName.Contains(textToSearch) ||
                            x.Phone.Contains(textToSearch)))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<PaginatedSupplierDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            string[] Words = Regex
                .Replace(textToSearch.Trim(), @"\s+", " ")
                .Split(" ");

            List<Supplier> ListSupplier = await DbContext.Supplier
                .AsNoTracking()
                .Where(x => strictSearch
                    ? Words.All(word =>
                        x.Name.Contains(word) ||
                        x.ContactName.Contains(word) ||
                        x.Email.Contains(word))
                    : Words.Any(word =>
                        x.Name.Contains(word) ||
                        x.ContactName.Contains(word) ||
                        x.Email.Contains(word)))
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            long TotalRecordsInDatabase = await DbContext.Supplier.LongCountAsync();
            int TotalRecordsInTheList = ListSupplier.Count;

            return new PaginatedSupplierDTO
            {
                ListSupplier             = ListSupplier,
                TotalRecordsInDatabase   = TotalRecordsInDatabase,
                TotalRecordsInTheList    = TotalRecordsInTheList,
                PageIndex                = pageIndex,
                PageSize                 = pageSize
            };
        }
        public async Task<PaginatedSupplierDTO> GetAllPaginatedBySearchAsync(
            string textToSearch,
            int pageIndex,
            int pageSize,
            bool? activeFilter = null,
            long storeId = 0)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            IQueryable<Supplier> query = DbContext.Supplier.AsNoTracking();

            if (storeId > 0)
                query = query.Where(x => x.StoreId == storeId);

            if (!string.IsNullOrWhiteSpace(textToSearch))
            {
                string[] Words = Regex
                    .Replace(textToSearch
                    .Trim(), @"\s+", " ")
                    .Split(" ");

                query = query.Where(x =>
                    Words.Any(word => x.Name.Contains(word)) ||
                    Words.Any(word => x.ContactName.Contains(word)) ||
                    Words.Any(word => x.Phone.Contains(word)) ||
                    Words.Any(word => x.Email.Contains(word)));
            }

            if (activeFilter.HasValue)
                query = query.Where(x => x.Active == activeFilter.Value);

            long totalFiltered = await query.LongCountAsync();

            IQueryable<Supplier> baseQuery = DbContext.Supplier.AsNoTracking();
            if (storeId > 0) baseQuery = baseQuery.Where(x => x.StoreId == storeId);
            long totalInDb = await baseQuery.LongCountAsync();

            List<Supplier> list = await query
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedSupplierDTO
            {
                ListSupplier           = list,
                TotalRecordsInDatabase = totalInDb,
                TotalRecordsInTheList  = list.Count,
                TotalFilteredRecords   = totalFiltered,
                PageIndex              = pageIndex,
                PageSize               = pageSize
            };
        }

        public async Task<int> CountActiveAsync(long storeId = 0)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            var query = DbContext.Supplier.AsNoTracking().Where(x => x.Active);
            if (storeId > 0) query = query.Where(x => x.StoreId == storeId);
            return await query.CountAsync();
        }
        #endregion

        #region Async Non-Queries
        public async Task<int> AddAsync(Supplier supplier)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            EntityEntry<Supplier> SupplierToAdd = await DbContext.Supplier.AddAsync(supplier);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> AddRangeAsync(List<Supplier> listSupplier)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            await DbContext.Supplier.AddRangeAsync(listSupplier);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Supplier supplier)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            DbContext.Supplier.Update(supplier);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteOneBySupplierIdAsync(long supplierId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier
                .Where(x => x.SupplierId == supplierId)
                .ExecuteDeleteAsync();
        }

        public async Task<int> DeleteAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Supplier.ExecuteDeleteAsync();
        }

        public async Task<int> DeleteManyAsync(List<Supplier> listSupplier)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            DbContext.Supplier.RemoveRange(listSupplier);

            return await DbContext.SaveChangesAsync();
        }
        #endregion

        #region Methods for DataTable
        public async Task<DataTable> GetAllInDataTableAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Supplier> ListSupplier = await DbContext.Supplier
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("SupplierId",   typeof(string));
            DataTable.Columns.Add("Name",         typeof(string));
            DataTable.Columns.Add("ContactName",  typeof(string));
            DataTable.Columns.Add("Phone",        typeof(string));
            DataTable.Columns.Add("Email",        typeof(string));
            DataTable.Columns.Add("Address",      typeof(string));
            DataTable.Columns.Add("TaxId",        typeof(string));
            DataTable.Columns.Add("Notes",        typeof(string));
            DataTable.Columns.Add("Active",       typeof(string));

            foreach (Supplier Supplier in ListSupplier)
            {
                DataTable.Rows.Add(
                    Supplier.SupplierId,
                    Supplier.Name,
                    Supplier.ContactName,
                    Supplier.Phone,
                    Supplier.Email,
                    Supplier.Address,
                    Supplier.TaxId,
                    Supplier.Notes,
                    Supplier.Active ? "Sí" : "No"
                );
            }

            return DataTable;
        }

        public async Task<DataTable> GetAllBySupplierIdInDataTableAsync(List<long> listCheckedSupplierIds)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Supplier> ListSupplier = await DbContext.Supplier
                .AsNoTracking()
                .Where(x => listCheckedSupplierIds.Contains(x.SupplierId))
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("SupplierId",   typeof(string));
            DataTable.Columns.Add("Name",         typeof(string));
            DataTable.Columns.Add("ContactName",  typeof(string));
            DataTable.Columns.Add("Phone",        typeof(string));
            DataTable.Columns.Add("Email",        typeof(string));
            DataTable.Columns.Add("Address",      typeof(string));
            DataTable.Columns.Add("TaxId",        typeof(string));
            DataTable.Columns.Add("Notes",        typeof(string));
            DataTable.Columns.Add("Active",       typeof(string));

            foreach (Supplier Supplier in ListSupplier)
            {
                DataTable.Rows.Add(
                    Supplier.SupplierId,
                    Supplier.Name,
                    Supplier.ContactName,
                    Supplier.Phone,
                    Supplier.Email,
                    Supplier.Address,
                    Supplier.TaxId,
                    Supplier.Notes,
                    Supplier.Active ? "Sí" : "No"
                );
            }

            return DataTable;
        }
        #endregion
    }
}
