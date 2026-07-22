using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Interfaces
{
    public partial interface ICashMovementRepository
    {
        Task AddAsync(CashMovement movement);
        Task<List<CashMovementListItem>> GetAllByStoreIdAsync(long storeId, int take = 50);
        Task<List<CashMovementListItem>> GetByStoreAndPeriodAsync(long storeId, DateTime dateFrom, DateTime dateTo);
    }
}
