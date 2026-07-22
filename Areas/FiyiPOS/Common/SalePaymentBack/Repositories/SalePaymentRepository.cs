using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Repositories
{
    public partial class SalePaymentRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ISalePaymentRepository
    {
        public async Task<List<SalePayment>> GetAllBySaleIdAsync(long saleId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SalePayment.AsNoTracking().Where(x => x.SaleId == saleId).ToListAsync();
        }
    }
}
