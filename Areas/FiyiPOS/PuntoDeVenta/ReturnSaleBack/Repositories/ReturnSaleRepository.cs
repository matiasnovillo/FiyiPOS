using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Interfaces;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleDetailBack.Entities;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Repositories
{
    public partial class ReturnSaleRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IReturnSaleRepository
    {
        public async Task<long> AddFullReturnAsync(ReturnSale header, List<ReturnSaleDetail> details)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();

            try
            {
                header.ReturnDate = DateTime.Now;
                header.CreatedAt  = DateTime.Now;
                header.Total      = details.Sum(d => d.Subtotal);

                await db.ReturnSale.AddAsync(header);
                await db.SaveChangesAsync();

                foreach (var d in details)
                    d.ReturnSaleId = header.ReturnSaleId;

                await db.ReturnSaleDetail.AddRangeAsync(details);
                await db.SaveChangesAsync();

                // Restore stock and log an InventoryMovement per line
                foreach (var d in details)
                {
                    var product = await db.Product.FirstOrDefaultAsync(p => p.ProductId == d.ProductId)
                        ?? throw new InvalidOperationException($"Producto {d.ProductNameSnapshot} no encontrado.");

                    decimal newStock = product.Stock + d.Quantity;

                    await db.Product
                        .Where(p => p.ProductId == d.ProductId)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, newStock));

                    await db.InventoryMovement.AddAsync(new InventoryMovement
                    {
                        ProductId    = d.ProductId,
                        SystemUserId = header.SystemUserId,
                        SaleId       = header.SaleId,
                        MovementType = "IN",
                        Quantity     = d.Quantity,
                        StockBefore  = product.Stock,
                        StockAfter   = newStock,
                        Reason       = "Devolución de venta",
                        Reference    = $"DEV-{header.ReturnSaleId}",
                        MovementDate = header.ReturnDate,
                    });
                }

                await db.SaveChangesAsync();
                await tx.CommitAsync();
                return header.ReturnSaleId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ReturnSaleLineDTO>> GetAllByStoreIdAsync(long storeId, int take = 60)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from r  in db.ReturnSale
                join s  in db.Sale on r.SaleId equals s.SaleId
                where r.StoreId == storeId
                orderby r.ReturnDate descending
                select new ReturnSaleLineDTO
                {
                    ReturnSaleId = r.ReturnSaleId,
                    TicketNumber = s.TicketNumber,
                    ReturnDate   = r.ReturnDate,
                    Reason       = r.Reason,
                    Total        = r.Total,
                    ItemCount    = db.ReturnSaleDetail.Count(d => d.ReturnSaleId == r.ReturnSaleId)
                }
            ).Take(take).ToListAsync();
        }

        public async Task<List<ReturnSaleDetail>> GetDetailsByReturnSaleIdAsync(long returnSaleId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.ReturnSaleDetail
                .AsNoTracking()
                .Where(d => d.ReturnSaleId == returnSaleId)
                .ToListAsync();
        }
    }
}
