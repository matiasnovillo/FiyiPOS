using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Interfaces;
using FiyiPOS.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Repositories
{
    public partial class InventoryMovementRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IInventoryMovementRepository
    {
        // Saves the movement and updates Product.Stock atomically inside one transaction.
        public async Task AddAsync(InventoryMovement movement)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            try
            {
                var product = await DbContext.Product.FirstOrDefaultAsync(p => p.ProductId == movement.ProductId)
                    ?? throw new InvalidOperationException("Producto no encontrado.");

                movement.StoreId      = product.StoreId;
                movement.StockBefore  = product.Stock;
                movement.MovementDate = DateTime.Now;

                // Reject decimal quantities for products that do not allow fractional sales.
                if (!product.AllowFractional && Math.Abs(movement.Quantity) != Math.Floor(Math.Abs(movement.Quantity)))
                    throw new InvalidOperationException(
                        "Este producto no permite venta fraccionada. La cantidad del movimiento debe ser un número entero.");

                decimal newStock = movement.MovementType switch
                {
                    "IN"     => product.Stock + movement.Quantity,
                    "OUT"    => product.Stock - movement.Quantity,
                    "ADJUST" => movement.Quantity,   // user enters new absolute value
                    _        => throw new ArgumentException("Tipo de movimiento inválido.")
                };

                // Normalize stored Quantity:
                // IN  → positive as entered
                // OUT → negate so the audit trail is readable (+entry / -exit)
                // ADJUST → store the delta (may be + or -)
                movement.Quantity = movement.MovementType switch
                {
                    "OUT"    => -Math.Abs(movement.Quantity),
                    "ADJUST" => newStock - product.Stock,
                    _        => movement.Quantity
                };

                movement.StockAfter = newStock;

                await DbContext.Product
                    .Where(p => p.ProductId == movement.ProductId)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, newStock));

                await DbContext.InventoryMovement.AddAsync(movement);
                await DbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginatedInventoryMovementDTO> GetAllPaginatedByStoreIdAsync(
            long storeId, string textToSearch, string movementTypeFilter, int pageIndex, int pageSize,
            DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            var baseQuery = from im in DbContext.InventoryMovement
                            join p in DbContext.Product on im.ProductId equals p.ProductId
                            where p.StoreId == storeId
                            select new { im, p };

            if (!string.IsNullOrWhiteSpace(textToSearch))
            {
                string[] Words = Regex
                    .Replace(textToSearch
                    .Trim(), @"\s+", " ")
                    .Split(" ");

                baseQuery = baseQuery
                    .Where(x => Words.Any(word => x.p.Name.Contains(word)) ||
                        Words.Any(word => x.p.Barcode.Contains(word)));
            }

            if (!string.IsNullOrWhiteSpace(movementTypeFilter))
                baseQuery = baseQuery.Where(x => x.im.MovementType == movementTypeFilter);

            if (dateFrom.HasValue)
                baseQuery = baseQuery.Where(x => x.im.MovementDate >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                baseQuery = baseQuery.Where(x => x.im.MovementDate < dateTo.Value.Date.AddDays(1));

            long totalInDb = await (from im in DbContext.InventoryMovement
                                    join p in DbContext.Product on im.ProductId equals p.ProductId
                                    where p.StoreId == storeId
                                    select im).LongCountAsync();

            long totalFiltered = await baseQuery.LongCountAsync();

            List<InventoryMovementLineDTO> list = await baseQuery
                .OrderByDescending(x => x.im.MovementDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new InventoryMovementLineDTO
                {
                    InventoryMovementId = x.im.InventoryMovementId,
                    ProductName         = x.p.Name,
                    Barcode             = x.p.Barcode,
                    MovementType        = x.im.MovementType,
                    Quantity            = x.im.Quantity,
                    StockBefore         = x.im.StockBefore,
                    StockAfter          = x.im.StockAfter,
                    Reason              = x.im.Reason,
                    Reference           = x.im.Reference,
                    MovementDate        = x.im.MovementDate
                })
                .ToListAsync();

            return new PaginatedInventoryMovementDTO
            {
                ListMovements          = list,
                TotalRecordsInDatabase = totalInDb,
                TotalRecordsInTheList  = list.Count,
                TotalFilteredRecords   = totalFiltered,
                PageIndex              = pageIndex,
                PageSize               = pageSize
            };
        }

        public async Task<(int total, int inCount, int outCount, int adjustCount)> GetCountsByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            var counts = await (
                from im in DbContext.InventoryMovement
                join p in DbContext.Product on im.ProductId equals p.ProductId
                where p.StoreId == storeId
                group im by im.MovementType into g
                select new { Type = g.Key, Count = g.Count() }
            ).ToListAsync();

            int inCount     = counts.FirstOrDefault(x => x.Type == "IN")?.Count     ?? 0;
            int outCount    = counts.FirstOrDefault(x => x.Type == "OUT")?.Count    ?? 0;
            int adjustCount = counts.FirstOrDefault(x => x.Type == "ADJUST")?.Count ?? 0;

            return (inCount + outCount + adjustCount, inCount, outCount, adjustCount);
        }

        // History for the store, most recent first, with product name joined.
        public async Task<List<InventoryMovementLineDTO>> GetAllByStoreIdAsync(long storeId, int take = 60)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await (
                from im in DbContext.InventoryMovement
                join p in DbContext.Product on im.ProductId equals p.ProductId
                where p.StoreId == storeId
                orderby im.MovementDate descending
                select new InventoryMovementLineDTO
                {
                    InventoryMovementId = im.InventoryMovementId,
                    ProductName         = p.Name,
                    Barcode             = p.Barcode,
                    MovementType        = im.MovementType,
                    Quantity            = im.Quantity,
                    StockBefore         = im.StockBefore,
                    StockAfter          = im.StockAfter,
                    Reason              = im.Reason,
                    Reference           = im.Reference,
                    MovementDate        = im.MovementDate
                }
            ).Take(take).ToListAsync();
        }
    }
}
