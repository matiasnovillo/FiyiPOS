using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Interfaces;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionDetailBack.Entities;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Repositories
{
    public partial class InternalConsumptionRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IInternalConsumptionRepository
    {
        public async Task<long> AddFullConsumptionAsync(InternalConsumption header, List<InternalConsumptionDetail> details)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();

            try
            {
                header.ConsumptionDate = DateTime.Now;
                header.CreatedAt       = DateTime.Now;
                header.TotalCost       = details.Sum(d => d.Subtotal);

                await db.InternalConsumption.AddAsync(header);
                await db.SaveChangesAsync();

                foreach (var d in details)
                    d.InternalConsumptionId = header.InternalConsumptionId;

                await db.InternalConsumptionDetail.AddRangeAsync(details);
                await db.SaveChangesAsync();

                // Decrement stock and log an InventoryMovement per line
                foreach (var d in details)
                {
                    var product = await db.Product.FirstOrDefaultAsync(p => p.ProductId == d.ProductId)
                        ?? throw new InvalidOperationException($"Producto {d.ProductNameSnapshot} no encontrado.");

                    decimal newStock = product.Stock - d.Quantity;

                    await db.Product
                        .Where(p => p.ProductId == d.ProductId)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, newStock));

                    await db.InventoryMovement.AddAsync(new InventoryMovement
                    {
                        StoreId      = header.StoreId,
                        ProductId    = d.ProductId,
                        SystemUserId = header.SystemUserId,
                        MovementType = "OUT",
                        Quantity     = -d.Quantity,
                        StockBefore  = product.Stock,
                        StockAfter   = newStock,
                        UnitCost     = d.UnitCost,
                        Reason       = "Consumo interno" + (string.IsNullOrWhiteSpace(header.Reason) ? "" : $": {header.Reason}"),
                        Reference    = $"CI-{header.InternalConsumptionId}",
                        MovementDate = header.ConsumptionDate,
                    });
                }

                await db.SaveChangesAsync();
                await tx.CommitAsync();
                return header.InternalConsumptionId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<InternalConsumptionLineDTO>> GetAllByStoreIdAsync(long storeId, int take = 200)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from c in db.InternalConsumption
                join u in db.SystemUser on c.SystemUserId equals u.SystemUserId
                where c.StoreId == storeId
                orderby c.ConsumptionDate descending
                select new InternalConsumptionLineDTO
                {
                    InternalConsumptionId = c.InternalConsumptionId,
                    ConsumptionDate       = c.ConsumptionDate,
                    Reason                = c.Reason,
                    OperatorName          = u.Name,
                    TotalCost             = c.TotalCost,
                    ItemCount             = db.InternalConsumptionDetail.Count(d => d.InternalConsumptionId == c.InternalConsumptionId),
                    TotalQuantity         = db.InternalConsumptionDetail.Where(d => d.InternalConsumptionId == c.InternalConsumptionId).Sum(d => (decimal?)d.Quantity) ?? 0,
                }
            ).Take(take).ToListAsync();
        }

        public async Task<List<InternalConsumptionDetail>> GetDetailsByInternalConsumptionIdAsync(long internalConsumptionId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.InternalConsumptionDetail
                .AsNoTracking()
                .Where(d => d.InternalConsumptionId == internalConsumptionId)
                .ToListAsync();
        }
    }
}
