using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Repositories
{
    public partial class CashMovementRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ICashMovementRepository
    {
        public async Task AddAsync(CashMovement movement)
        {
            await using var db = await _factory.CreateDbContextAsync();
            movement.MovementDate = DateTime.Now;
            await db.CashMovement.AddAsync(movement);
            await db.SaveChangesAsync();
        }

        public async Task<List<CashMovementListItem>> GetAllByStoreIdAsync(long storeId, int take = 50)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from m in db.CashMovement
                join u in db.SystemUser on m.SystemUserId equals u.SystemUserId
                where m.StoreId == storeId
                orderby m.MovementDate descending
                select new CashMovementListItem
                {
                    CashMovementId = m.CashMovementId,
                    Type           = m.Type,
                    Amount         = m.Amount,
                    Reason         = m.Reason,
                    MovementDate   = m.MovementDate,
                    OperatorName   = u.Name,
                }
            ).Take(take).ToListAsync();
        }

        public async Task<List<CashMovementListItem>> GetByStoreAndPeriodAsync(long storeId, DateTime dateFrom, DateTime dateTo)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.CashMovement
                .AsNoTracking()
                .Where(m => m.StoreId == storeId && m.MovementDate >= dateFrom && m.MovementDate < dateTo)
                .Join(db.SystemUser,
                      m => m.SystemUserId,
                      u => u.SystemUserId,
                      (m, u) => new CashMovementListItem
                      {
                          CashMovementId = m.CashMovementId,
                          Type           = m.Type,
                          Amount         = m.Amount,
                          Reason         = m.Reason,
                          MovementDate   = m.MovementDate,
                          OperatorName   = u.Name,
                      })
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();
        }
    }
}
