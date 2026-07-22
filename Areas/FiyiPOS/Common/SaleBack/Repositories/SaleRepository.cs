using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Interfaces;
using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Entities;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Repositories
{
    public partial class SaleRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ISaleRepository
    {
        public async Task<long> AddFullSaleAsync(Sale sale, List<SaleDetail> details, List<SalePayment> payments, List<(long ProductId, decimal Quantity)>? comboStockLines = null)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            try
            {
                // Read folio config from store-specific config
                string prefix = (await DbContext.Config
                    .Where(c => c.ConfigKey == "folio_prefix" && c.StoreId == sale.StoreId)
                    .Select(c => c.Value)
                    .FirstOrDefaultAsync()) ?? "T";

                int padding = int.TryParse(
                    await DbContext.Config
                        .Where(c => c.ConfigKey == "folio_padding" && c.StoreId == sale.StoreId)
                        .Select(c => c.Value)
                        .FirstOrDefaultAsync(),
                    out int f) ? f : 6;

                // MAX(SaleId)+1 never recedes even if sales are deleted
                long next = (await DbContext.Sale.MaxAsync(x => (long?)x.SaleId) ?? 0L) + 1;

                sale.TicketNumber = $"{prefix}-{next.ToString().PadLeft(padding, '0')}";
                sale.SaleDate     = DateTime.Now;
                sale.CreatedAt    = DateTime.Now;

                await DbContext.Sale.AddAsync(sale);
                await DbContext.SaveChangesAsync();   // populates SaleId

                foreach (var d in details)  d.SaleId = sale.SaleId;
                foreach (var p in payments) p.SaleId = sale.SaleId;

                await DbContext.SaleDetail.AddRangeAsync(details);
                await DbContext.SalePayment.AddRangeAsync(payments);
                await DbContext.SaveChangesAsync();

                // Decrement stock for regular lines (skip combo placeholders with ProductId == null)
                foreach (var d in details.Where(d => d.ProductId != null))
                {
                    await DbContext.Product
                        .Where(p => p.ProductId == d.ProductId)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - d.Quantity));
                }

                // Decrement stock for each combo component
                if (comboStockLines != null)
                {
                    foreach (var (productId, qty) in comboStockLines)
                    {
                        await DbContext.Product
                            .Where(p => p.ProductId == productId)
                            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - qty));
                    }
                }

                await transaction.CommitAsync();
                return sale.SaleId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Sale?> GetOneBySaleIdAsync(long saleId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Sale.AsNoTracking().FirstOrDefaultAsync(x => x.SaleId == saleId);
        }

        public async Task<List<Sale>> GetAllByStoreIdAsync(long storeId, int take = 50)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.Sale
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderByDescending(x => x.SaleDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task<SalesSummaryReport> GetSalesSummaryAsync(long storeId, DateTime from, DateTime to)
        {
            await using var db = await _factory.CreateDbContextAsync();
            DateTime dateFrom = from;
            DateTime toExcl = to.Date.AddDays(1);

            var agg = await db.Sale
                .Where(s => s.StoreId == storeId && s.SaleDate >= dateFrom && s.SaleDate < toExcl)
                .GroupBy(_ => 1)
                .Select(g => new { Total = g.Sum(s => s.Total), TaxAmount = g.Sum(s => s.TaxAmount), Count = g.Count() })
                .FirstOrDefaultAsync();

            var byPayment = await (
                from sp in db.SalePayment
                join s  in db.Sale          on sp.SaleId          equals s.SaleId
                join pm in db.PaymentMethod on sp.PaymentMethodId  equals pm.PaymentMethodId
                where s.StoreId == storeId && s.SaleDate >= dateFrom && s.SaleDate < toExcl
                group sp.Amount by pm.Name into g
                select new PaymentMethodLine
                {
                    MethodName = g.Key,
                    Count      = g.Count(),
                    Total      = g.Sum(a => a)
                }
            ).ToListAsync();

            decimal total = agg?.Total ?? 0;
            decimal tax   = agg?.TaxAmount ?? 0;
            int     count = agg?.Count ?? 0;

            return new SalesSummaryReport
            {
                TotalRevenue    = total,
                TotalTaxAmount  = tax,
                SalesCount      = count,
                AverageTicket   = count > 0 ? Math.Round(total / count, 2) : 0,
                ByPaymentMethod = byPayment,
            };
        }

        public async Task<List<TopProductLine>> GetTopProductsAsync(long storeId, DateTime from, DateTime to, int top = 20)
        {
            await using var db = await _factory.CreateDbContextAsync();
            DateTime dateFrom = from;
            DateTime toExcl = to.Date.AddDays(1);

            return await (
                from sd in db.SaleDetail
                join s  in db.Sale on sd.SaleId equals s.SaleId
                where s.StoreId == storeId && s.SaleDate >= dateFrom && s.SaleDate < toExcl
                group sd by sd.ProductNameSnapshot into g
                select new TopProductLine
                {
                    ProductName = g.Key,
                    UnitsSold   = g.Sum(x => x.Quantity),
                    Revenue     = g.Sum(x => x.Subtotal)
                }
            ).OrderByDescending(x => x.UnitsSold).Take(top).ToListAsync();
        }

        public async Task<List<OperatorSalesLine>> GetSalesByOperatorAsync(long storeId, DateTime from, DateTime to)
        {
            await using var db = await _factory.CreateDbContextAsync();
            DateTime dateFrom = from;
            DateTime toExcl = to.Date.AddDays(1);

            return await (
                from s in db.Sale
                join u in db.SystemUser on s.SystemUserId equals u.SystemUserId
                where s.StoreId == storeId && s.SaleDate >= dateFrom && s.SaleDate < toExcl
                group s by u.Name into g
                select new OperatorSalesLine
                {
                    OperatorName = g.Key,
                    SalesCount   = g.Count(),
                    TotalSold    = g.Sum(x => x.Total)
                }
            ).OrderByDescending(x => x.TotalSold).ToListAsync();
        }

        public async Task<Sale?> GetOneByTicketNumberAsync(long storeId, string ticketNumber)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Sale.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StoreId == storeId && s.TicketNumber == ticketNumber);
        }

        public async Task<List<SaleDetail>> GetDetailsBySaleIdAsync(long saleId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.SaleDetail.AsNoTracking()
                .Where(d => d.SaleId == saleId)
                .ToListAsync();
        }

        public async Task<List<SaleListLine>> GetSalesByDateRangeAsync(long storeId, DateTime from, DateTime to)
        {
            await using var db = await _factory.CreateDbContextAsync();
            DateTime dateFrom = from;
            DateTime toExcl   = to.Date.AddDays(1);

            return await (
                from s  in db.Sale
                join u  in db.SystemUser on s.SystemUserId equals u.SystemUserId
                join cl in db.Client on (long?)s.ClientId equals (long?)cl.ClientId into clg
                from cl in clg.DefaultIfEmpty()
                where s.StoreId == storeId && s.SaleDate >= dateFrom && s.SaleDate < toExcl
                orderby s.SaleDate descending
                select new SaleListLine
                {
                    SaleId       = s.SaleId,
                    TicketNumber = s.TicketNumber,
                    SaleDate     = s.SaleDate,
                    OperatorName = u.Name,
                    ClientName   = cl != null ? cl.Nombres + " " + cl.Apellidos : "",
                    Total        = s.Total,
                    IsCredit     = s.AmountPaid == 0 && s.Total > 0,
                }
            ).ToListAsync();
        }

        public async Task<string> GetPaymentMethodNameBySaleIdAsync(long saleId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from sp in db.SalePayment
                join pm in db.PaymentMethod on sp.PaymentMethodId equals pm.PaymentMethodId
                where sp.SaleId == saleId
                select pm.Name
            ).FirstOrDefaultAsync() ?? "Crédito";
        }

        public async Task<List<ProductCategoryGroup>> GetProductsByCategoryAsync(long storeId, DateTime dateFrom, DateTime dateTo)
        {
            await using var db = await _factory.CreateDbContextAsync();

            // Step 1: flat detail lines for the period
            var detailLines = await db.SaleDetail
                .AsNoTracking()
                .Join(db.Sale,
                      sd => sd.SaleId,
                      s  => s.SaleId,
                      (sd, s) => new { sd, s })
                .Where(x => x.s.StoreId == storeId
                            && x.s.SaleDate >= dateFrom
                            && x.s.SaleDate < dateTo)
                .Select(x => new
                {
                    x.sd.ProductId,
                    x.sd.ProductNameSnapshot,
                    x.sd.Quantity,
                    x.sd.Subtotal,
                })
                .ToListAsync();

            // Step 2: look up category for each distinct product ID
            long[] productIds = detailLines
                .Where(l => l.ProductId.HasValue)
                .Select(l => l.ProductId!.Value)
                .Distinct()
                .ToArray();

            var catByProduct = await db.Product
                .AsNoTracking()
                .Where(p => productIds.Contains(p.ProductId))
                .GroupJoin(db.Category,
                           p   => p.CategoryId,
                           cat => cat.CategoryId,
                           (p, cats) => new { p.ProductId, Cat = cats.FirstOrDefault() })
                .Select(x => new { x.ProductId, CategoryName = x.Cat != null ? x.Cat.Name : "Sin categoría" })
                .ToListAsync();

            var catDict = catByProduct.ToDictionary(x => x.ProductId, x => x.CategoryName);

            // Step 3: group in memory — "Sin categoría" always last
            return detailLines
                .GroupBy(l => l.ProductId.HasValue
                    ? catDict.GetValueOrDefault(l.ProductId.Value, "Sin categoría")
                    : "Sin categoría")
                .OrderBy(g => g.Key == "Sin categoría" ? "\xFF" : g.Key)
                .Select(g => new ProductCategoryGroup
                {
                    CategoryName = g.Key,
                    Lines = g.GroupBy(l => l.ProductNameSnapshot)
                              .OrderBy(pg => pg.Key)
                              .Select(pg => new ProductCategoryLine
                              {
                                  ProductName   = pg.Key,
                                  TotalQty      = pg.Sum(l => l.Quantity),
                                  TotalSubtotal = pg.Sum(l => l.Subtotal),
                              })
                              .ToList()
                })
                .ToList();
        }

        public async Task<List<ProfitabilityLine>> GetProfitabilityAsync(long storeId, DateTime dateFrom, DateTime dateTo)
        {
            await using var db = await _factory.CreateDbContextAsync();

            // Step 1: flat sale detail lines for the period
            var raw = await db.SaleDetail
                .AsNoTracking()
                .Join(db.Sale,
                      sd => sd.SaleId,
                      s  => s.SaleId,
                      (sd, s) => new { sd, s })
                .Where(x => x.s.StoreId == storeId
                            && x.s.SaleDate >= dateFrom
                            && x.s.SaleDate < dateTo)
                .Select(x => new
                {
                    x.sd.ProductId,
                    x.sd.PromotionId,
                    x.sd.ProductNameSnapshot,
                    x.sd.Quantity,
                    x.sd.Subtotal,
                    x.sd.CostPriceSnapshot,
                })
                .ToListAsync();

            if (!raw.Any()) return new();

            // Step 2a: cost price + category for each distinct regular product
            long[] productIds = raw
                .Where(r => r.ProductId.HasValue)
                .Select(r => r.ProductId!.Value)
                .Distinct()
                .ToArray();

            var productInfo = await db.Product
                .AsNoTracking()
                .Where(p => productIds.Contains(p.ProductId))
                .GroupJoin(db.Category,
                           p   => p.CategoryId,
                           cat => cat.CategoryId,
                           (p, cats) => new { p.ProductId, p.CostPrice, Cat = cats.FirstOrDefault() })
                .Select(x => new
                {
                    x.ProductId,
                    x.CostPrice,
                    CategoryName = x.Cat != null ? x.Cat.Name : "Sin categoría"
                })
                .ToDictionaryAsync(x => x.ProductId);

            // Step 2b: cost per combo unit = sum(component.Quantity × component.CostPrice)
            long[] promoIds = raw
                .Where(r => r.PromotionId.HasValue)
                .Select(r => r.PromotionId!.Value)
                .Distinct()
                .ToArray();

            Dictionary<long, decimal> comboCostPerUnit = new();
            if (promoIds.Length > 0)
            {
                var comboComponents = await db.PromotionDetail
                    .AsNoTracking()
                    .Where(pd => promoIds.Contains(pd.PromotionId))
                    .Join(db.Product.AsNoTracking(),
                          pd => pd.ProductId,
                          p  => p.ProductId,
                          (pd, p) => new { pd.PromotionId, Cost = pd.Quantity * p.CostPrice })
                    .ToListAsync();

                comboCostPerUnit = comboComponents
                    .GroupBy(x => x.PromotionId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Cost));
            }

            // Step 3: group by product name in memory, compute margin
            return raw
                .GroupBy(r => r.ProductNameSnapshot)
                .Select(g =>
                {
                    var    first   = g.First();
                    long?  pid     = first.ProductId;
                    long?  promoId = first.PromotionId;
                    var    info    = pid.HasValue && productInfo.TryGetValue(pid.Value, out var v) ? v : null;
                    decimal costU  = info?.CostPrice ?? 0;
                    string  cat    = info?.CategoryName ?? "Sin categoría";

                    decimal qty     = g.Sum(x => x.Quantity);
                    decimal revenue = g.Sum(x => x.Subtotal);
                    decimal cogs    = g.Sum(x =>
                    {
                        if (x.CostPriceSnapshot.HasValue) return x.Quantity * x.CostPriceSnapshot.Value;
                        if (x.ProductId.HasValue)         return x.Quantity * costU;
                        if (x.PromotionId.HasValue && comboCostPerUnit.TryGetValue(x.PromotionId.Value, out var cc))
                            return x.Quantity * cc;
                        return 0m;
                    });
                    decimal profit  = revenue - cogs;

                    return new ProfitabilityLine
                    {
                        ProductName  = g.Key,
                        CategoryName = cat,
                        TotalQty     = qty,
                        TotalRevenue = revenue,
                        TotalCost    = cogs,
                        GrossProfit  = profit,
                        MarginPct    = revenue > 0 ? Math.Round(profit / revenue * 100, 1) : 0m
                    };
                })
                .OrderBy(l => l.CategoryName == "Sin categoría" ? "\xFF" : l.CategoryName)
                .ThenByDescending(l => l.GrossProfit)
                .ToList();
        }
    }
}
