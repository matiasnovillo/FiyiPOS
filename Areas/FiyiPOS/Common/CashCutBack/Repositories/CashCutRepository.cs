using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Interfaces;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutDetailBack.Entities;
using FiyiPOS.DatabaseContexts;
using static FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.DTOs.CashCutReportDTO;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Repositories
{
    public partial class CashCutRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ICashCutRepository
    {
        // Returns a calculated snapshot of the period since the last cut.
        // Nothing is saved — the UI shows this for review before the user confirms.
        public async Task<CashCutPreviewDTO> GetPreviewAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            // Period starts right after the last closed cut (or epoch if none)
            DateTime periodStart = await DbContext.CashCut
                .AsNoTracking()
                .Where(x => x.StoreId == storeId && x.ClosedAt != null)
                .OrderByDescending(x => x.ClosedAt)
                .Select(x => x.ClosedAt!.Value)
                .FirstOrDefaultAsync();

            if (periodStart == default)
                periodStart = new DateTime(2000, 1, 1);

            DateTime periodEnd = DateTime.Now;

            // Sales in the period for this store
            var salesInPeriod = await DbContext.Sale
                .AsNoTracking()
                .Where(s => s.StoreId == storeId && s.SaleDate >= periodStart)
                .ToListAsync();

            int    salesCount   = salesInPeriod.Count;
            long[] saleIds      = salesInPeriod.Select(s => s.SaleId).ToArray();
            decimal totalRevenue = salesInPeriod.Sum(s => s.Total);

            // Aggregate SalePayments joined with PaymentMethod for those sales
            var salePaymentLines = await (
                from sp in DbContext.SalePayment
                join pm in DbContext.PaymentMethod on sp.PaymentMethodId equals pm.PaymentMethodId
                where saleIds.Contains(sp.SaleId)
                group new { sp, pm } by new { sp.PaymentMethodId, pm.Name, pm.IsCash } into g
                select new CashCutDetailLineDTO
                {
                    PaymentMethodId   = g.Key.PaymentMethodId,
                    PaymentMethodName = g.Key.Name,
                    IsCash            = g.Key.IsCash,
                    TotalCollected    = g.Sum(x => x.sp.Amount),
                    TransactionCount  = g.Count()
                }
            ).ToListAsync();

            // Cash movements in the period, joined with PaymentMethod
            var movements = await (
                from m in DbContext.CashMovement
                join pm in DbContext.PaymentMethod on m.PaymentMethodId equals pm.PaymentMethodId
                where m.StoreId == storeId && m.MovementDate >= periodStart && m.MovementDate <= periodEnd
                select new { m.Type, m.Amount, pm.PaymentMethodId, pm.Name, pm.IsCash }
            ).ToListAsync();

            // Solo los movimientos en efectivo físico afectan el monto esperado en la gaveta
            decimal totalCashIn  = movements.Where(m => m.IsCash && m.Type == "IN").Sum(m => m.Amount);
            decimal totalCashOut = movements.Where(m => m.IsCash && m.Type == "OUT").Sum(m => m.Amount);

            // Las entradas manuales por métodos no-efectivo (transferencia, tarjeta, etc.)
            // se suman a la recaudación por forma de pago, igual que una venta por ese medio.
            var nonCashMovementLines = movements
                .Where(m => !m.IsCash && m.Type == "IN")
                .GroupBy(m => new { m.PaymentMethodId, m.Name })
                .Select(g => new CashCutDetailLineDTO
                {
                    PaymentMethodId   = g.Key.PaymentMethodId,
                    PaymentMethodName = g.Key.Name,
                    IsCash            = false,
                    TotalCollected    = g.Sum(x => x.Amount),
                    TransactionCount  = g.Count()
                });

            var lines = salePaymentLines
                .Concat(nonCashMovementLines)
                .GroupBy(l => new { l.PaymentMethodId, l.PaymentMethodName, l.IsCash })
                .Select(g => new CashCutDetailLineDTO
                {
                    PaymentMethodId   = g.Key.PaymentMethodId,
                    PaymentMethodName = g.Key.PaymentMethodName,
                    IsCash            = g.Key.IsCash,
                    TotalCollected    = g.Sum(x => x.TotalCollected),
                    TransactionCount  = g.Sum(x => x.TransactionCount)
                })
                .ToList();

            return new CashCutPreviewDTO
            {
                PeriodStart  = periodStart,
                PeriodEnd    = periodEnd,
                SalesCount   = salesCount,
                TotalRevenue = totalRevenue,
                TotalCashIn  = totalCashIn,
                TotalCashOut = totalCashOut,
                Lines        = lines
            };
        }

        // Saves the cut atomically: CashCut header first (to get the ID), then the detail lines.
        public async Task<long> CreateAsync(CashCut cashCut, List<CashCutDetail> details)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            try
            {
                cashCut.ClosedAt = DateTime.Now;

                await DbContext.CashCut.AddAsync(cashCut);
                await DbContext.SaveChangesAsync();   // populates CashCutId

                foreach (var d in details)
                    d.CashCutId = cashCut.CashCutId;

                await DbContext.CashCutDetail.AddRangeAsync(details);
                await DbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return cashCut.CashCutId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<CashCutReportDTO> GetReportDataAsync(long cashCutId, string storeName)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var cut = await db.CashCut
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CashCutId == cashCutId)
                ?? throw new InvalidOperationException($"CashCut {cashCutId} not found.");

            DateTime periodFrom = cut.OpenedAt;
            DateTime periodTo   = cut.ClosedAt!.Value;

            string operatorName = await db.SystemUser
                .AsNoTracking()
                .Where(u => u.SystemUserId == cut.SystemUserId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync() ?? "";

            // Payment breakdown
            var paymentLines = await (
                from d in db.CashCutDetail
                join pm in db.PaymentMethod on d.PaymentMethodId equals pm.PaymentMethodId
                where d.CashCutId == cashCutId
                orderby pm.IsCash descending
                select new PaymentLine(pm.Name, pm.IsCash, d.TransactionCount, d.TotalCollected)
            ).ToListAsync();

            // Cash movements in the period
            var movements = await (
                from m in db.CashMovement
                join u in db.SystemUser on m.SystemUserId equals u.SystemUserId
                join pm in db.PaymentMethod on m.PaymentMethodId equals pm.PaymentMethodId
                where m.StoreId == cut.StoreId
                      && m.MovementDate >= periodFrom
                      && m.MovementDate <= periodTo
                orderby m.MovementDate
                select new MovementLine(m.Type, m.Amount, m.Reason, u.Name, m.MovementDate, pm.Name)
            ).ToListAsync();

            // Sales in the period
            var salesRaw = await (
                from s in db.Sale
                join u in db.SystemUser on s.SystemUserId equals u.SystemUserId
                join cl in db.Client on s.ClientId equals cl.ClientId into clGroup
                from cl in clGroup.DefaultIfEmpty()
                where s.StoreId == cut.StoreId
                      && s.SaleDate >= periodFrom
                      && s.SaleDate <= periodTo
                orderby s.SaleDate
                select new
                {
                    s.SaleId,
                    s.TicketNumber,
                    s.SaleDate,
                    s.Total,
                    s.TaxAmount,
                    OperatorName = u.Name,
                    ClientName   = cl != null ? cl.Nombres + " " + cl.Apellidos : "",
                }
            ).ToListAsync();

            long[] saleIds = salesRaw.Select(s => s.SaleId).ToArray();

            decimal totalSalesGross = salesRaw.Sum(s => s.Total);
            decimal totalSalesNet   = totalSalesGross - salesRaw.Sum(s => s.TaxAmount);

            var pmBySale = await (
                from sp in db.SalePayment
                join pm in db.PaymentMethod on sp.PaymentMethodId equals pm.PaymentMethodId
                where saleIds.Contains(sp.SaleId)
                select new { sp.SaleId, pm.Name }
            ).ToListAsync();

            var creditSaleIds = await db.Credit
                .AsNoTracking()
                .Where(c => c.SaleId != null && saleIds.Contains(c.SaleId!.Value))
                .Select(c => c.SaleId!.Value)
                .ToListAsync();

            var sales = salesRaw.Select(s => new SaleLine(
                s.TicketNumber,
                s.SaleDate,
                s.OperatorName,
                s.ClientName.Trim(),
                s.Total,
                pmBySale.FirstOrDefault(p => p.SaleId == s.SaleId)?.Name ?? "—",
                creditSaleIds.Contains(s.SaleId)
            )).ToList();

            // Returns in the period
            var returns = await (
                from r in db.ReturnSale
                join s in db.Sale on r.SaleId equals s.SaleId
                join u in db.SystemUser on r.SystemUserId equals u.SystemUserId
                where r.StoreId == cut.StoreId
                      && r.ReturnDate >= periodFrom
                      && r.ReturnDate <= periodTo
                orderby r.ReturnDate
                select new ReturnLine(s.TicketNumber, r.ReturnDate, r.Reason ?? "", r.Total, u.Name)
            ).ToListAsync();

            // Credits created in the period
            var credits = await (
                from cr in db.Credit
                join cl in db.Client on cr.ClientId equals cl.ClientId
                where cr.StoreId == cut.StoreId
                      && cr.CreatedAt >= periodFrom
                      && cr.CreatedAt <= periodTo
                orderby cr.CreatedAt
                select new CreditLine(
                    cl.Nombres + " " + cl.Apellidos,
                    cr.OriginalAmount,
                    cr.Balance,
                    cr.CreatedAt)
            ).ToListAsync();

            // Payment method lookup by sale (reuse pmBySale built above)
            var pmNameBySale = pmBySale
                .GroupBy(p => p.SaleId)
                .ToDictionary(g => g.Key, g => g.First().Name);

            // Products sold — step 1: get flat detail lines for the period (includes SaleId for payment breakdown)
            var detailLines = await (
                from sd in db.SaleDetail
                join s in db.Sale on sd.SaleId equals s.SaleId
                where s.StoreId == cut.StoreId
                      && s.SaleDate >= periodFrom
                      && s.SaleDate <= periodTo
                select new
                {
                    sd.ProductId,
                    sd.ProductNameSnapshot,
                    sd.Quantity,
                    sd.Subtotal,
                    sd.SaleId,
                }
            ).ToListAsync();

            // Step 2: look up category for each distinct product
            long[] productIds = detailLines
                .Where(l => l.ProductId.HasValue)
                .Select(l => l.ProductId!.Value)
                .Distinct()
                .ToArray();

            var catByProduct = await (
                from p in db.Product
                join cat in db.Category on p.CategoryId equals cat.CategoryId into catGroup
                from cat in catGroup.DefaultIfEmpty()
                where productIds.Contains(p.ProductId)
                select new { p.ProductId, CategoryName = cat != null ? cat.Name : "Sin categoría" }
            ).ToListAsync();

            var catDict = catByProduct.ToDictionary(x => x.ProductId, x => x.CategoryName);

            // Step 3: group in memory, computing product lines and payment breakdown per category
            var productsByCategory = detailLines
                .GroupBy(l => l.ProductId.HasValue
                    ? catDict.GetValueOrDefault(l.ProductId.Value, "Sin categoría")
                    : "Sin categoría")
                .OrderBy(g => g.Key == "Sin categoría" ? "\xFF" : g.Key)
                .Select(g => new ProductCategoryGroup(
                    g.Key,
                    g.GroupBy(l => l.ProductNameSnapshot)
                     .OrderBy(pg => pg.Key)
                     .Select(pg => new ProductSoldLine(
                         pg.Key,
                         pg.Sum(l => l.Quantity),
                         pg.Sum(l => l.Subtotal)))
                     .ToList(),
                    g.GroupBy(l => pmNameBySale.GetValueOrDefault(l.SaleId, ""))
                     .Where(pg => !string.IsNullOrEmpty(pg.Key))
                     .OrderByDescending(pg => pg.Sum(l => l.Subtotal))
                     .Select(pg => new PaymentMethodBreakdown(pg.Key, pg.Sum(l => l.Subtotal)))
                     .ToList()))
                .ToList();

            // COGS — productos individuales vendidos en el período
            var cogsLines = await db.SaleDetail
                .AsNoTracking()
                .Join(db.Sale,
                      sd => sd.SaleId,
                      s  => s.SaleId,
                      (sd, s) => new { sd, s })
                .Where(x => x.s.StoreId == cut.StoreId
                            && x.s.SaleDate >= periodFrom
                            && x.s.SaleDate <= periodTo
                            && x.sd.ProductId.HasValue)
                .Join(db.Product,
                      x  => x.sd.ProductId!.Value,
                      p  => p.ProductId,
                      (x, p) => new { x.sd.Quantity, x.sd.CostPriceSnapshot, p.CostPrice })
                .ToListAsync();

            decimal regularCOGS = cogsLines.Sum(x => x.Quantity * (x.CostPriceSnapshot ?? x.CostPrice));

            // COGS — combos/promociones: SaleDetail.PromotionId → PromotionDetail → Product.CostPrice
            var comboCOGSLines = await db.SaleDetail
                .AsNoTracking()
                .Join(db.Sale,
                      sd => sd.SaleId,
                      s  => s.SaleId,
                      (sd, s) => new { sd, s })
                .Where(x => x.s.StoreId == cut.StoreId
                            && x.s.SaleDate >= periodFrom
                            && x.s.SaleDate <= periodTo
                            && x.sd.PromotionId.HasValue)
                .Join(db.PromotionDetail,
                      x  => x.sd.PromotionId!.Value,
                      pd => pd.PromotionId,
                      (x, pd) => new { SaleQty = x.sd.Quantity, pd.ProductId, ComponentQty = pd.Quantity })
                .Join(db.Product,
                      x  => x.ProductId,
                      p  => p.ProductId,
                      (x, p) => new { x.SaleQty, x.ComponentQty, p.CostPrice })
                .ToListAsync();

            decimal comboCOGS = comboCOGSLines.Sum(x => x.SaleQty * x.ComponentQty * x.CostPrice);
            decimal totalCOGS = regularCOGS + comboCOGS;

            // Purchases in the period
            var purchasesRaw = await (
                from p in db.Purchase
                join s in db.Supplier on p.SupplierId equals s.SupplierId
                where p.StoreId == cut.StoreId
                      && p.PurchaseDate >= periodFrom
                      && p.PurchaseDate <= periodTo
                orderby p.PurchaseDate
                select new
                {
                    p.PurchaseId,
                    SupplierName    = s.Name,
                    p.SupplierInvoice,
                    p.PurchaseDate,
                    p.Subtotal,
                    p.TaxAmount,
                    p.Total,
                    p.IsConfirmed
                }
            ).ToListAsync();

            long[] purchaseIds = purchasesRaw.Select(p => p.PurchaseId).ToArray();
            var itemCounts = purchaseIds.Length > 0
                ? await db.PurchaseDetail
                    .Where(d => purchaseIds.Contains(d.PurchaseId))
                    .GroupBy(d => d.PurchaseId)
                    .Select(g => new { PurchaseId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.PurchaseId, x => x.Count)
                : new Dictionary<long, int>();

            var purchases = purchasesRaw.Select(p => new PurchaseLine(
                p.SupplierName,
                p.SupplierInvoice,
                p.PurchaseDate,
                itemCounts.GetValueOrDefault(p.PurchaseId, 0),
                p.Subtotal,
                p.TaxAmount,
                p.Total,
                p.IsConfirmed
            )).ToList();

            // Consumo interno en el período, agrupado por operador
            var consumptionRaw = await (
                from ic  in db.InternalConsumption
                join icd in db.InternalConsumptionDetail on ic.InternalConsumptionId equals icd.InternalConsumptionId
                join u   in db.SystemUser on ic.SystemUserId equals u.SystemUserId
                where ic.StoreId == cut.StoreId
                      && ic.ConsumptionDate >= periodFrom
                      && ic.ConsumptionDate <= periodTo
                select new
                {
                    OperatorName = u.Name,
                    ic.ConsumptionDate,
                    ic.Reason,
                    icd.ProductNameSnapshot,
                    icd.Quantity,
                    icd.Subtotal,
                }
            ).ToListAsync();

            var internalConsumptionByOperator = consumptionRaw
                .GroupBy(x => x.OperatorName)
                .OrderByDescending(g => g.Sum(x => x.Subtotal))
                .Select(g => new InternalConsumptionGroup(
                    g.Key,
                    g.OrderBy(x => x.ConsumptionDate)
                     .Select(x => new ConsumptionLine(x.ConsumptionDate, x.Reason, x.ProductNameSnapshot, x.Quantity, x.Subtotal))
                     .ToList(),
                    g.Sum(x => x.Subtotal)))
                .ToList();

            return new CashCutReportDTO
            {
                StoreName          = storeName,
                OperatorName       = operatorName,
                GeneratedAt        = DateTime.Now,
                CashCutId          = cut.CashCutId,
                OpenedAt           = cut.OpenedAt,
                ClosedAt           = periodTo,
                OpeningAmount      = cut.OpeningAmount,
                ExpectedAmount     = cut.ExpectedAmount,
                ActualAmount       = cut.ActualAmount,
                Difference         = cut.Difference,
                TotalSales         = cut.TotalSales,
                TotalCOGS          = totalCOGS,
                TotalSalesGross    = totalSalesGross,
                TotalSalesNet      = totalSalesNet,
                Notes              = cut.Notes,
                PaymentLines       = paymentLines,
                Movements          = movements,
                Sales              = sales,
                Returns            = returns,
                Credits            = credits,
                ProductsByCategory = productsByCategory,
                Purchases          = purchases,
                InternalConsumptionByOperator = internalConsumptionByOperator,
            };
        }

        // History list, most recent first.
        public async Task<List<CashCut>> GetAllByStoreIdAsync(long storeId, int take = 30)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.CashCut
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderByDescending(x => x.ClosedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetUncountedSalesCountAsync(long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            DateTime lastCut = await db.CashCut
                .AsNoTracking()
                .Where(x => x.StoreId == storeId && x.ClosedAt != null)
                .OrderByDescending(x => x.ClosedAt)
                .Select(x => x.ClosedAt!.Value)
                .FirstOrDefaultAsync();

            if (lastCut == default)
                lastCut = new DateTime(2000, 1, 1);

            return await db.Sale
                .AsNoTracking()
                .CountAsync(s => s.StoreId == storeId && s.SaleDate >= lastCut);
        }

        public async Task<(List<CashCut> Items, int TotalCount, int TotalSales, int ShortfallCount, int SurplusCount)>
            GetPagedAsync(long storeId, int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo, string? filter)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var q = db.CashCut.AsNoTracking().Where(x => x.StoreId == storeId);

            if (dateFrom.HasValue)
                q = q.Where(x => x.ClosedAt >= dateFrom.Value);
            if (dateTo.HasValue)
                q = q.Where(x => x.ClosedAt < dateTo.Value.Date.AddDays(1));
            if (!string.IsNullOrWhiteSpace(filter))
                q = q.Where(x => x.Notes != null && x.Notes.Contains(filter));

            int total      = await q.CountAsync();
            int totalSales = total > 0 ? await q.SumAsync(x => x.TotalSales) : 0;
            int shortfall  = await q.CountAsync(x => x.Difference < 0);
            int surplus    = await q.CountAsync(x => x.Difference > 0);

            var items = await q
                .OrderByDescending(x => x.ClosedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total, totalSales, shortfall, surplus);
        }
    }
}
