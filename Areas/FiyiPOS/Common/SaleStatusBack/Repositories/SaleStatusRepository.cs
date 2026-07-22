using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Repositories
{
    public partial class SaleStatusRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : ISaleStatusRepository
    {
        public async Task<List<SaleStatus>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SaleStatus.AsNoTracking().ToListAsync();
        }
    }
}
