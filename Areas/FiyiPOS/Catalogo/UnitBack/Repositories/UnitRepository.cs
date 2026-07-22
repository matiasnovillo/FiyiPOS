using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Interfaces;
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

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Repositories
{
    public partial class UnitRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IUnitRepository
    {
        #region Async Queries
        public async Task<int> CountAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Unit.AsNoTracking().CountAsync();
        }

        public async Task<Unit?> GetOneByUnitIdAsync(long unitId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Unit
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UnitId == unitId);
        }

        public async Task<List<Unit>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Unit
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Unit>> GetAllByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Unit
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Unit>> GetAllActiveAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Unit
                .AsNoTracking()
                .Where(x => x.Active && x.StoreId == storeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<PaginatedUnitDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            string[] Words = Regex
                .Replace(textToSearch.Trim(), @"\s+", " ")
                .Split(" ");

            List<Unit> ListUnit = await DbContext.Unit
                .AsNoTracking()
                .Where(x => strictSearch
                    ? Words.All(word => x.Name.Contains(word) || x.Abbreviation.Contains(word))
                    : Words.Any(word => x.Name.Contains(word) || x.Abbreviation.Contains(word)))
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            long TotalRecordsInDatabase = await DbContext.Unit.LongCountAsync();

            return new PaginatedUnitDTO
            {
                ListUnit                = ListUnit,
                TotalRecordsInDatabase  = TotalRecordsInDatabase,
                TotalRecordsInTheList   = ListUnit.Count,
                PageIndex               = pageIndex,
                PageSize                = pageSize
            };
        }
        #endregion

        #region Async Non-Queries
        public async Task<int> AddAsync(Unit unit)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            EntityEntry<Unit> UnitToAdd = await DbContext.Unit.AddAsync(unit);

            return await DbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Unit> units)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            await DbContext.Unit.AddRangeAsync(units);
            await DbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Unit unit)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            DbContext.Unit.Update(unit);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteOneByUnitIdAsync(long unitId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Unit
                .Where(x => x.UnitId == unitId)
                .ExecuteDeleteAsync();
        }

        public async Task<int> DeleteAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            return await DbContext.Unit.ExecuteDeleteAsync();
        }
        #endregion

        #region Methods for DataTable
        public async Task<DataTable> GetAllInDataTableAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Unit> ListUnit = await DbContext.Unit
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("UnitId",       typeof(string));
            DataTable.Columns.Add("Name",         typeof(string));
            DataTable.Columns.Add("Abbreviation", typeof(string));

            foreach (Unit Unit in ListUnit)
            {
                DataTable.Rows.Add(
                    Unit.UnitId,
                    Unit.Name,
                    Unit.Abbreviation
                );
            }

            return DataTable;
        }
        #endregion
    }
}
