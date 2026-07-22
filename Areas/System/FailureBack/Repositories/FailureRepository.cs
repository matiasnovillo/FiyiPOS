using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.Areas.System.FailureBack.Entities;
using FiyiPOS.Areas.System.FailureBack.Interfaces;
using FiyiPOS.DatabaseContexts;
using FiyiPOS.Mailing;
using System.Text.RegularExpressions;
using System.Data;

/*
 * 
 * Coded by fiyistack.com
 * Copyright © 2026
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 */

namespace FiyiPOS.Areas.System.FailureBack.Repositories
{
    public partial class FailureRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory,
        IServiceScopeFactory               _scopeFactory) : IFailureRepository
    {
        public async Task<int> AddAsync(Failure failure)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            EntityEntry<Failure> FailureToAdd = await DbContext.Failure.AddAsync(failure);

            int AffectedRowsNumber = await DbContext.SaveChangesAsync();

            // Fire-and-forget email alert — scope propio para que no dependa del scope HTTP actual
            _ = Task.Run(async () =>
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var email = scope.ServiceProvider.GetRequiredService<Email>();
                    await email.SendErrorAlertAsync(
                        failure.Message,
                        failure.Source,
                        failure.StackTrace,
                        failure.DateTimeCreation);
                }
                catch { /* nunca propagar: el email falla silenciosamente */ }
            });

            return AffectedRowsNumber;
        }
    }
}