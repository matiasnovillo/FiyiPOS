using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Repositories
{
    public partial class SaleDetailRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ISaleDetailRepository
    {
        public async Task<List<SaleDetail>> GetAllBySaleIdAsync(long saleId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SaleDetail.AsNoTracking().Where(x => x.SaleId == saleId).ToListAsync();
        }
    }
}
