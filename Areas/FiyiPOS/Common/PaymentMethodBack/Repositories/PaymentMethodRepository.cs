using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Repositories
{
    public partial class PaymentMethodRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IPaymentMethodRepository
    {
        public async Task<List<PaymentMethod>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.PaymentMethod.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<List<PaymentMethod>> GetAllActiveAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.PaymentMethod.AsNoTracking().Where(x => x.Active).OrderBy(x => x.Name).ToListAsync();
        }
    }
}
