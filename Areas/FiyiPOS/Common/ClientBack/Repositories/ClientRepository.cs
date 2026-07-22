using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Interfaces;
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

namespace FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Repositories
{
    public partial class ClientRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IClientRepository
    {
        #region Async Queries
        public async Task<int> CountAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            int RowsNumber = await DbContext.Client
                .AsNoTracking()
                .CountAsync();

            return RowsNumber;
        }

        public async Task<Client?> GetOneByClientIdAsync(long clientId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            Client? Client = await DbContext.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ClientId == clientId);

            return Client;
        }

        public async Task<List<Client>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Client> ListClient = await DbContext.Client
                .AsNoTracking()
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .ToListAsync();

            return ListClient;
        }

        public async Task<List<Client>> GetAllByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Client
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .ToListAsync();
        }

        public async Task<List<Client>> GetAllByClientIdCheckedAsync(List<long> listCheckedClientIds)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Client> ListClient = await DbContext.Client
                .AsNoTracking()
                .Where(x => listCheckedClientIds.Contains(x.ClientId))
                .ToListAsync();

            return ListClient;
        }

        public async Task<List<Client>> GetAllForModalAsync(string textToSearch)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Client> ListClient = await DbContext.Client
                .AsNoTracking()
                .Where(x => x.Nombres.Contains(textToSearch) ||
                            x.Apellidos.Contains(textToSearch) ||
                            x.Correo.Contains(textToSearch))
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .ToListAsync();

            return ListClient;
        }

        public async Task<PaginatedClientDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch,
            int pageIndex,
            int pageSize)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            string[] Words = Regex
                .Replace(textToSearch.Trim(), @"\s+", " ")
                .Split(" ");

            List<Client> ListClient = await DbContext.Client
                .AsNoTracking()
                .Where(x => strictSearch
                    ? Words.All(word =>
                        x.Nombres.Contains(word) ||
                        x.Apellidos.Contains(word) ||
                        x.Correo.Contains(word))
                    : Words.Any(word =>
                        x.Nombres.Contains(word) ||
                        x.Apellidos.Contains(word) ||
                        x.Correo.Contains(word)))
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            long TotalRecordsInDatabase = await DbContext.Client.LongCountAsync();

            int TotalRecordsInTheList = ListClient.Count;

            return new PaginatedClientDTO
            {
                ListClient = ListClient,
                TotalRecordsInDatabase = TotalRecordsInDatabase,
                TotalRecordsInTheList = TotalRecordsInTheList,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<PaginatedClientDTO> GetAllPaginatedByStoreIdAsync(
            long storeId,
            string textToSearch,
            int pageIndex,
            int pageSize,
            List<long>? clientIdFilter = null)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            var query = DbContext.Client
                .AsNoTracking()
                .Where(x => x.StoreId == storeId);

            if (clientIdFilter != null && clientIdFilter.Count > 0)
                query = query.Where(x => clientIdFilter.Contains(x.ClientId));

            if (!string.IsNullOrWhiteSpace(textToSearch))
            {
                string[] words = Regex.Replace(textToSearch.Trim(), @"\s+", " ").Split(" ");

                query = query.Where(x => words.Any(word =>
                    x.Nombres.Contains(word)   ||
                    x.Apellidos.Contains(word) ||
                    x.Correo.Contains(word)    ||
                    x.Telefono.Contains(word)));
            }

            long totalFiltered  = await query.LongCountAsync();
            long totalInDb      = await DbContext.Client.AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .LongCountAsync();

            var list = await query
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedClientDTO
            {
                ListClient             = list,
                TotalRecordsInDatabase = totalInDb,
                TotalRecordsInTheList  = list.Count,
                TotalFilteredRecords   = totalFiltered,
                PageIndex              = pageIndex,
                PageSize               = pageSize,
            };
        }

        public async Task<int> CountWithCreditByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Client
                .AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.TieneCreditoAutorizado);
        }
        #endregion

        #region Async Non-Queries
        public async Task<int> AddAsync(Client client)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            EntityEntry<Client> ClientToAdd = await DbContext.Client.AddAsync(client);

            int AffectedRowsNumber = await DbContext.SaveChangesAsync();

            return AffectedRowsNumber;
        }

        public async Task<int> AddRangeAsync(List<Client> listClient)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            await DbContext.Client.AddRangeAsync(listClient);

            int AffectedRowsNumber = await DbContext.SaveChangesAsync();

            return AffectedRowsNumber;
        }

        public async Task<int> UpdateAsync(Client client)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            DbContext.Client.Update(client);

            int AffectedRowsNumber = await DbContext.SaveChangesAsync();

            return AffectedRowsNumber;
        }

        public async Task<int> DeleteOneByClientIdAsync(long clientId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            int AffectedRowsNumber = await DbContext.Client
                .Where(x => x.ClientId == clientId)
                .ExecuteDeleteAsync();

            return AffectedRowsNumber;
        }

        public async Task<int> DeleteAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            int AffectedRowsNumber = await DbContext.Client
                .ExecuteDeleteAsync();

            return AffectedRowsNumber;
        }

        public async Task<int> DeleteManyAsync(List<Client> listClient)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            DbContext.Client.RemoveRange(listClient);

            int AffectedRows = await DbContext.SaveChangesAsync();

            return AffectedRows;
        }
        #endregion

        #region Methods for DataTable
        public async Task<DataTable> GetAllInDataTableByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Client> ListClient = await DbContext.Client
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("ClientId",                 typeof(string));
            DataTable.Columns.Add("Nombres",                  typeof(string));
            DataTable.Columns.Add("Apellidos",                typeof(string));
            DataTable.Columns.Add("Telefono",                 typeof(string));
            DataTable.Columns.Add("Correo",                   typeof(string));
            DataTable.Columns.Add("Domicilio",                typeof(string));
            DataTable.Columns.Add("CUIT",                     typeof(string));
            DataTable.Columns.Add("Notas",                    typeof(string));
            DataTable.Columns.Add("TieneCreditoAutorizado",   typeof(string));
            DataTable.Columns.Add("LimiteCredito",            typeof(string));
            DataTable.Columns.Add("SaldoCredito",             typeof(string));
            DataTable.Columns.Add("Active",                   typeof(string));

            foreach (Client Client in ListClient)
            {
                DataTable.Rows.Add(
                    Client.ClientId,
                    Client.Nombres,
                    Client.Apellidos,
                    Client.Telefono,
                    Client.Correo,
                    Client.Domicilio,
                    Client.CUIT,
                    Client.Notas,
                    Client.TieneCreditoAutorizado,
                    Client.LimiteCredito,
                    Client.SaldoCredito,
                    Client.Active
                );
            }

            return DataTable;
        }

        public async Task<DataTable> GetAllByClientIdInDataTableAsync(List<long> listCheckedClientIds)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("ClientId",                 typeof(string));
            DataTable.Columns.Add("DateTimeCreation",         typeof(string));
            DataTable.Columns.Add("DateTimeLastModification", typeof(string));
            DataTable.Columns.Add("UserCreationId",           typeof(string));
            DataTable.Columns.Add("UserLastModificationId",   typeof(string));
            DataTable.Columns.Add("Active",                   typeof(string));
            DataTable.Columns.Add("Nombres",                  typeof(string));
            DataTable.Columns.Add("Apellidos",                typeof(string));
            DataTable.Columns.Add("Telefono",                 typeof(string));
            DataTable.Columns.Add("Correo",                   typeof(string));
            DataTable.Columns.Add("Domicilio",                typeof(string));
            DataTable.Columns.Add("RFC",                      typeof(string));
            DataTable.Columns.Add("Notas",                    typeof(string));
            DataTable.Columns.Add("TieneCreditoAutorizado",   typeof(string));
            DataTable.Columns.Add("LimiteCredito",            typeof(string));
            DataTable.Columns.Add("SaldoCredito",             typeof(string));

            List<Client> ListCheckedClient = await DbContext.Client
                .AsNoTracking()
                .Where(x => listCheckedClientIds.Contains(x.ClientId))
                .ToListAsync();

            foreach (Client Client in ListCheckedClient)
            {
                if (Client != null)
                {
                    DataTable.Rows.Add(
                        Client.ClientId,
                        Client.DateTimeCreation,
                        Client.DateTimeLastModification,
                        Client.UserCreationId,
                        Client.UserLastModificationId,
                        Client.Active,
                        Client.Nombres,
                        Client.Apellidos,
                        Client.Telefono,
                        Client.Correo,
                        Client.Domicilio,
                        Client.CUIT,
                        Client.Notas,
                        Client.TieneCreditoAutorizado,
                        Client.LimiteCredito,
                        Client.SaldoCredito
                    );
                }
            }

            return DataTable;
        }

        public async Task<DataTable> GetAllInDataTableAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            List<Client> ListClient = await DbContext.Client
                .AsNoTracking()
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .ToListAsync();

            DataTable DataTable = new();
            DataTable.Columns.Add("ClientId",                 typeof(string));
            DataTable.Columns.Add("DateTimeCreation",         typeof(string));
            DataTable.Columns.Add("DateTimeLastModification", typeof(string));
            DataTable.Columns.Add("UserCreationId",           typeof(string));
            DataTable.Columns.Add("UserLastModificationId",   typeof(string));
            DataTable.Columns.Add("Active",                   typeof(string));
            DataTable.Columns.Add("Nombres",                  typeof(string));
            DataTable.Columns.Add("Apellidos",                typeof(string));
            DataTable.Columns.Add("Telefono",                 typeof(string));
            DataTable.Columns.Add("Correo",                   typeof(string));
            DataTable.Columns.Add("Domicilio",                typeof(string));
            DataTable.Columns.Add("RFC",                      typeof(string));
            DataTable.Columns.Add("Notas",                    typeof(string));
            DataTable.Columns.Add("TieneCreditoAutorizado",   typeof(string));
            DataTable.Columns.Add("LimiteCredito",            typeof(string));
            DataTable.Columns.Add("SaldoCredito",             typeof(string));

            foreach (Client Client in ListClient)
            {
                DataTable.Rows.Add(
                    Client.ClientId,
                    Client.DateTimeCreation,
                    Client.DateTimeLastModification,
                    Client.UserCreationId,
                    Client.UserLastModificationId,
                    Client.Active,
                    Client.Nombres,
                    Client.Apellidos,
                    Client.Telefono,
                    Client.Correo,
                    Client.Domicilio,
                    Client.CUIT,
                    Client.Notas,
                    Client.TieneCreditoAutorizado,
                    Client.LimiteCredito,
                    Client.SaldoCredito
                );
            }

            return DataTable;
        }
        #endregion
    }
}
