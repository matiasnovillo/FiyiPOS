using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Repositories
{
    public class CreditRepository(IDbContextFactory<FiyiPOSDbContext> _factory) : ICreditRepository
    {
        public async Task<List<CreditLineDTO>> GetAllByStoreIdAsync(long storeId, bool onlyOpen = true)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var query =
                from cr in db.Credit
                join cl in db.Client on cr.ClientId equals cl.ClientId
                where cr.StoreId == storeId
                select new { cr, cl };

            if (onlyOpen)
                query = query.Where(x => x.cr.Active);

            return await query
                .OrderBy(x => x.cr.DueDate.HasValue ? 0 : 1)   // overdue/dated first
                .ThenBy(x => x.cr.DueDate)
                .ThenByDescending(x => x.cr.CreatedAt)
                .Select(x => new CreditLineDTO
                {
                    CreditId       = x.cr.CreditId,
                    ClientId       = x.cl.ClientId,
                    ClientName     = x.cl.Nombres + " " + x.cl.Apellidos,
                    ClientPhone    = x.cl.Telefono,
                    OriginalAmount = x.cr.OriginalAmount,
                    Balance        = x.cr.Balance,
                    DueDate        = x.cr.DueDate,
                    Active         = x.cr.Active,
                    CreatedAt      = x.cr.CreatedAt,
                    Notes          = x.cr.Notes,
                })
                .ToListAsync();
        }

        // Sum of all open (unpaid) credit balances for a given client.
        public async Task<decimal> GetOpenBalanceByClientIdAsync(long clientId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Credit
                .Where(c => c.ClientId == clientId && c.Active)
                .SumAsync(c => (decimal?)c.Balance) ?? 0m;
        }

        public async Task AddAsync(Credit credit, long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            credit.StoreId   = storeId;
            credit.CreatedAt = DateTime.Now;
            credit.Balance   = credit.OriginalAmount;
            credit.Active    = true;
            await db.Credit.AddAsync(credit);
            await db.SaveChangesAsync();
        }
    }
}
