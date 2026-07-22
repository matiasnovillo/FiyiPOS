using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Interfaces
{
    public partial interface IInventoryMovementRepository
    {
        Task AddAsync(InventoryMovement movement);
        Task<List<InventoryMovementLineDTO>> GetAllByStoreIdAsync(long storeId, int take = 60);
        Task<PaginatedInventoryMovementDTO> GetAllPaginatedByStoreIdAsync(long storeId, string textToSearch, string movementTypeFilter, int pageIndex, int pageSize, DateTime? dateFrom = null, DateTime? dateTo = null);
        Task<(int total, int inCount, int outCount, int adjustCount)> GetCountsByStoreIdAsync(long storeId);
    }
}
