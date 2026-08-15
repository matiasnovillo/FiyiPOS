using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Interfaces
{
    public partial interface ISystemUserRepository
    {
        Task<int> CountAsync();
        Task<SystemUser?> GetOneBySystemUserIdAsync(long systemUserId);
        Task<SystemUser?> GetOneByUsernameAndPasswordAsync(string username, string passwordHash);
        Task<List<SystemUser>> GetAllAsync();
        Task<List<SystemUser>> GetAllByStoreIdAsync(long storeId);
        Task<PaginatedSystemUserDTO> GetAllPaginatedAsync(string textToSearch, bool strictSearch, int pageIndex, int pageSize);
        Task<int> AddAsync(SystemUser systemUser);
        Task<int> UpdateAsync(SystemUser systemUser);
        Task<int> DeleteOneBySystemUserIdAsync(long systemUserId);
        Task ToggleActiveAsync(long systemUserId);

        Task<SystemUser?> GetOneByUsernameAsync(string username);
        Task<SystemUser?> GetOneByActivationTokenAsync(string token);
        Task ActivateByTokenAsync(string token);

        Task SetPasswordResetTokenAsync(long systemUserId, string token, DateTime expiresAt);
        Task<SystemUser?> GetOneByPasswordResetTokenAsync(string token);
        Task<bool> ResetPasswordByTokenAsync(string token, string newPasswordHash);
    }
}
