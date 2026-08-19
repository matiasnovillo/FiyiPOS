using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Interfaces;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseDetailBack.Entities;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Repositories
{
    public partial class PurchaseRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IPurchaseRepository
    {
        public async Task<List<PurchaseLineDTO>> GetAllLinesByStoreIdAsync(long storeId, int take = 50)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from p in db.Purchase
                join s in db.Supplier on p.SupplierId equals s.SupplierId
                where p.StoreId == storeId
                orderby p.PurchaseDate descending
                select new PurchaseLineDTO
                {
                    PurchaseId      = p.PurchaseId,
                    SupplierId      = p.SupplierId,
                    SystemUserId    = p.SystemUserId,
                    SupplierName    = s.Name,
                    SupplierInvoice = p.SupplierInvoice,
                    PurchaseDate    = p.PurchaseDate,
                    Subtotal        = p.Subtotal,
                    TaxAmount       = p.TaxAmount,
                    Total           = p.Total,
                    Notes           = p.Notes,
                    IsConfirmed     = p.IsConfirmed
                }
            ).Take(take).ToListAsync();
        }

        public async Task<List<PurchaseLineDTO>> GetByPeriodAsync(long storeId, DateTime dateFrom, DateTime dateTo)
        {
            DateTime start = dateFrom.Date;
            DateTime end   = dateTo.Date.AddDays(1);
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from p in db.Purchase
                join s in db.Supplier on p.SupplierId equals s.SupplierId
                where p.StoreId == storeId
                      && p.PurchaseDate >= start
                      && p.PurchaseDate < end
                orderby p.PurchaseDate descending
                select new PurchaseLineDTO
                {
                    PurchaseId      = p.PurchaseId,
                    SupplierId      = p.SupplierId,
                    SystemUserId    = p.SystemUserId,
                    SupplierName    = s.Name,
                    SupplierInvoice = p.SupplierInvoice,
                    PurchaseDate    = p.PurchaseDate,
                    Subtotal        = p.Subtotal,
                    TaxAmount       = p.TaxAmount,
                    Total           = p.Total,
                    Notes           = p.Notes,
                    IsConfirmed     = p.IsConfirmed
                }
            ).ToListAsync();
        }

        public async Task<List<PurchaseDetailLineDTO>> GetDetailsByPurchaseIdAsync(long purchaseId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from d in db.PurchaseDetail
                join p in db.Product on d.ProductId equals p.ProductId
                where d.PurchaseId == purchaseId
                select new PurchaseDetailLineDTO
                {
                    PurchaseDetailId = d.PurchaseDetailId,
                    ProductId        = d.ProductId,
                    ProductName      = p.Name,
                    Barcode          = p.Barcode,
                    Quantity         = d.Quantity,
                    UnitCost         = d.UnitCost,
                    Subtotal         = d.Subtotal
                }
            ).ToListAsync();
        }

        public async Task<long> AddFullPurchaseAsync(Purchase purchase, List<PurchaseDetail> details)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                purchase.CreatedAt   = DateTime.Now;
                purchase.IsConfirmed = false;

                await db.Purchase.AddAsync(purchase);
                await db.SaveChangesAsync();   // populates PurchaseId

                foreach (var d in details)
                {
                    d.PurchaseId = purchase.PurchaseId;
                    d.Subtotal   = d.Quantity * d.UnitCost;
                }

                await db.PurchaseDetail.AddRangeAsync(details);
                await db.SaveChangesAsync();

                await tx.CommitAsync();
                return purchase.PurchaseId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task ConfirmAsync(long purchaseId, long systemUserId, string paymentMethod)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                var purchase = await db.Purchase.FirstOrDefaultAsync(p => p.PurchaseId == purchaseId)
                    ?? throw new InvalidOperationException("Compra no encontrada.");

                if (purchase.IsConfirmed)
                    throw new InvalidOperationException("Esta compra ya fue confirmada.");

                var details = await db.PurchaseDetail
                    .Where(d => d.PurchaseId == purchaseId)
                    .ToListAsync();

                // Mark confirmed
                await db.Purchase
                    .Where(p => p.PurchaseId == purchaseId)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsConfirmed, true));

                var now = DateTime.Now;
                foreach (var detail in details)
                {
                    var product = await db.Product.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId)
                        ?? throw new InvalidOperationException($"Producto {detail.ProductId} no encontrado.");

                    if (!product.AllowFractional && detail.Quantity != Math.Floor(detail.Quantity))
                        throw new InvalidOperationException(
                            $"El producto \"{product.Name}\" no admite cantidades fraccionadas " +
                            $"(tiene {detail.Quantity:N2} u). Editá la compra antes de confirmarla.");

                    decimal newStock = product.Stock + detail.Quantity;

                    await db.Product
                        .Where(p => p.ProductId == detail.ProductId)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, newStock));

                    await db.InventoryMovement.AddAsync(new InventoryMovement
                    {
                        StoreId      = purchase.StoreId,
                        ProductId    = detail.ProductId,
                        SystemUserId = systemUserId,
                        SupplierId   = purchase.SupplierId,
                        PurchaseId   = purchaseId,
                        MovementType = "IN",
                        Quantity     = detail.Quantity,
                        StockBefore  = product.Stock,
                        StockAfter   = newStock,
                        UnitCost     = detail.UnitCost,
                        Reference    = purchase.SupplierInvoice,
                        Reason       = $"Compra #{purchaseId}",
                        MovementDate = now
                    });
                }

                if (paymentMethod == "CASH")
                {
                    string supplierName = await db.Supplier
                        .AsNoTracking()
                        .Where(s => s.SupplierId == purchase.SupplierId)
                        .Select(s => s.Name)
                        .FirstOrDefaultAsync() ?? "";

                    long cashMethodId = await db.PaymentMethod.AsNoTracking()
                        .Where(pm => pm.IsCash)
                        .Select(pm => pm.PaymentMethodId)
                        .FirstOrDefaultAsync();

                    await db.CashMovement.AddAsync(new CashMovement
                    {
                        StoreId         = purchase.StoreId,
                        SystemUserId    = systemUserId,
                        PaymentMethodId = cashMethodId,
                        Type            = "OUT",
                        Amount          = purchase.Total,
                        Reason          = $"Pago compra #{purchaseId} · {supplierName}",
                        MovementDate    = now
                    });
                }

                await db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
