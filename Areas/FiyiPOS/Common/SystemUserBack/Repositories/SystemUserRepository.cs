using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FiyiPOS.DatabaseContexts;
using FiyiPOS.Mailing;
using System.Text.RegularExpressions;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Interfaces;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Repositories
{
    public partial class SystemUserRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory,
        IServiceScopeFactory _scopeFactory) : ISystemUserRepository
    {
        public async Task<int> CountAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser.AsNoTracking().CountAsync();
        }

        public async Task<SystemUser?> GetOneBySystemUserIdAsync(long systemUserId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SystemUserId == systemUserId);
        }

        public async Task<SystemUser?> GetOneByUsernameAndPasswordAsync(string username, string passwordHash)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .FirstOrDefaultAsync(x => (x.Username == username || x.Name == username) &&
                                          x.PasswordHash == passwordHash &&
                                          x.Active);
        }

        public async Task<List<SystemUser>> GetAllAsync()
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<SystemUser>> GetAllByStoreIdAsync(long storeId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .Where(u => DbContext.SystemUserStore
                    .Any(sus => sus.SystemUserId == u.SystemUserId && sus.StoreId == storeId))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<PaginatedSystemUserDTO> GetAllPaginatedAsync(string textToSearch,
            bool strictSearch, int pageIndex, int pageSize)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            string[] Words = Regex
                .Replace(textToSearch.Trim(), @"\s+", " ")
                .Split(" ");

            List<SystemUser> ListSystemUser = await DbContext.SystemUser
                .AsNoTracking()
                .Where(x => strictSearch
                    ? Words.All(word => x.Name.Contains(word) || x.Username.Contains(word))
                    : Words.Any(word => x.Name.Contains(word) || x.Username.Contains(word)))
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            long TotalRecordsInDatabase = await DbContext.SystemUser.LongCountAsync();

            return new PaginatedSystemUserDTO
            {
                ListSystemUser         = ListSystemUser,
                TotalRecordsInDatabase = TotalRecordsInDatabase,
                TotalRecordsInTheList  = ListSystemUser.Count,
                PageIndex              = pageIndex,
                PageSize               = pageSize
            };
        }

        public async Task<int> AddAsync(SystemUser systemUser)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            EntityEntry<SystemUser> ToAdd = await DbContext.SystemUser.AddAsync(systemUser);
            int result = await DbContext.SaveChangesAsync();

            string name  = systemUser.Name;
            string email = systemUser.Username;
            string role  = systemUser.Role;
            DateTime now = DateTime.Now;

            _ = Task.Run(async () =>
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<Email>();
                    await emailService.SendNewUserAlertAsync(name, email, role, now);
                }
                catch { }
            });

            return result;
        }

        public async Task<int> UpdateAsync(SystemUser systemUser)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            DbContext.SystemUser.Update(systemUser);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteOneBySystemUserIdAsync(long systemUserId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .Where(x => x.SystemUserId == systemUserId)
                .ExecuteDeleteAsync();
        }

        public async Task<SystemUser?> GetOneByUsernameAsync(string username)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<SystemUser?> GetOneByUsernameOrNameAsync(string usernameOrName)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == usernameOrName || x.Name == usernameOrName);
        }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeSystemUserId = null)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AnyAsync(x => x.Name == name &&
                               (excludeSystemUserId == null || x.SystemUserId != excludeSystemUserId));
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser.AnyAsync(x => x.Username == username);
        }

        public async Task<bool> IsActiveByUsernameAsync(string username)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser.AnyAsync(x => x.Username == username && x.Active);
        }

        public async Task<SystemUser?> GetOneByActivationTokenAsync(string token)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ActivationToken == token);
        }

        public async Task ActivateByTokenAsync(string token)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            await DbContext.SystemUser
                .Where(x => x.ActivationToken == token)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Active,          true)
                    .SetProperty(u => u.ActivationToken, (string?)null));
        }

        public async Task ToggleActiveAsync(long systemUserId)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            bool current = await DbContext.SystemUser
                .Where(u => u.SystemUserId == systemUserId)
                .Select(u => u.Active)
                .FirstOrDefaultAsync();
            await DbContext.SystemUser
                .Where(u => u.SystemUserId == systemUserId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.Active, !current));
        }

        public async Task SetPasswordResetTokenAsync(long systemUserId, string token, DateTime expiresAt)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            await DbContext.SystemUser
                .Where(x => x.SystemUserId == systemUserId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.PasswordResetToken,         token)
                    .SetProperty(u => u.PasswordResetTokenExpiresAt, expiresAt));
        }

        public async Task<SystemUser?> GetOneByPasswordResetTokenAsync(string token)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();
            return await DbContext.SystemUser
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PasswordResetToken == token);
        }

        public async Task<bool> ResetPasswordByTokenAsync(string token, string newPasswordHash)
        {
            await using var DbContext = await _factory.CreateDbContextAsync();

            SystemUser? user = await DbContext.SystemUser
                .FirstOrDefaultAsync(x => x.PasswordResetToken == token);

            if (user == null || user.PasswordResetTokenExpiresAt == null ||
                user.PasswordResetTokenExpiresAt < DateTime.Now)
            {
                return false;
            }

            user.PasswordHash                 = newPasswordHash;
            user.PasswordResetToken            = null;
            user.PasswordResetTokenExpiresAt   = null;
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
