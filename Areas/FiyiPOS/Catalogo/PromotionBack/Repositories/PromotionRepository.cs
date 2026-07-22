using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Interfaces;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionDetailBack.Entities;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Repositories
{
    public partial class PromotionRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IPromotionRepository
    {
        public async Task<List<PromotionVM>> GetAllActiveByStoreIdAsync(long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await BuildVMsAsync(db, storeId, activeOnly: true);
        }

        public async Task<List<PromotionVM>> GetAllByStoreIdAsync(long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await BuildVMsAsync(db, storeId, activeOnly: false);
        }

        public async Task<long> SaveAsync(Promotion promotion, List<PromotionDetail> details)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                bool isNew = promotion.PromotionId == 0;
                if (isNew)
                {
                    promotion.CreatedAt = DateTime.Now;
                    await db.Promotion.AddAsync(promotion);
                    await db.SaveChangesAsync();
                }
                else
                {
                    var existing = await db.Promotion.FindAsync(promotion.PromotionId);
                    if (existing == null) throw new InvalidOperationException("Promoción no encontrada.");
                    existing.Name        = promotion.Name;
                    existing.Description = promotion.Description;
                    existing.Price       = promotion.Price;
                    existing.Active      = promotion.Active;
                    await db.SaveChangesAsync();
                }

                // Replace details: delete all then re-insert
                await db.PromotionDetail
                    .Where(d => d.PromotionId == promotion.PromotionId)
                    .ExecuteDeleteAsync();

                foreach (var d in details)
                    d.PromotionId = promotion.PromotionId;

                if (details.Any())
                    await db.PromotionDetail.AddRangeAsync(details);

                await db.SaveChangesAsync();
                await tx.CommitAsync();
                return promotion.PromotionId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(long promotionId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                await db.PromotionDetail.Where(d => d.PromotionId == promotionId).ExecuteDeleteAsync();
                await db.Promotion.Where(p => p.PromotionId == promotionId).ExecuteDeleteAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private static async Task<List<PromotionVM>> BuildVMsAsync(FiyiPOSDbContext db, long storeId, bool activeOnly)
        {
            var query = db.Promotion.AsNoTracking().Where(p => p.StoreId == storeId);
            if (activeOnly) query = query.Where(p => p.Active);

            var promos = await query.OrderBy(p => p.Name).ToListAsync();
            if (!promos.Any()) return new();

            var ids     = promos.Select(p => p.PromotionId).ToList();
            var details = await db.PromotionDetail.AsNoTracking()
                .Where(d => ids.Contains(d.PromotionId))
                .ToListAsync();

            var componentProductIds = details.Select(d => d.ProductId).Distinct().ToList();
            var productInfo = await db.Product.AsNoTracking()
                .Where(p => componentProductIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.Stock, p.AllowFractional, p.UnitId })
                .ToDictionaryAsync(p => p.ProductId);

            var unitIds = productInfo.Values.Where(p => p.UnitId.HasValue).Select(p => p.UnitId!.Value).Distinct().ToList();
            var unitAbbr = await db.Unit.AsNoTracking()
                .Where(u => unitIds.Contains(u.UnitId))
                .ToDictionaryAsync(u => u.UnitId, u => u.Abbreviation);

            return promos.Select(p => new PromotionVM
            {
                PromotionId = p.PromotionId,
                Name        = p.Name,
                Description = p.Description,
                Price       = p.Price,
                Active      = p.Active,
                Details     = details
                    .Where(d => d.PromotionId == p.PromotionId)
                    .Select(d =>
                    {
                        var prod = productInfo.GetValueOrDefault(d.ProductId);
                        string abbr = prod?.UnitId.HasValue == true
                            ? unitAbbr.GetValueOrDefault(prod.UnitId!.Value, "")
                            : "";
                        return new PromotionDetailVM
                        {
                            ProductId        = d.ProductId,
                            ProductName      = d.ProductNameSnapshot,
                            Quantity         = d.Quantity,
                            Stock            = prod?.Stock ?? 0,
                            AllowFractional  = prod?.AllowFractional ?? false,
                            UnitAbbreviation = abbr,
                        };
                    }).ToList(),
            }).ToList();
        }
    }
}
