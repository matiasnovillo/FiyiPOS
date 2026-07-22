using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<List<PurchaseLineDTO>>       GetAllLinesByStoreIdAsync(long storeId, int take = 50);
        Task<List<PurchaseLineDTO>>       GetByPeriodAsync(long storeId, DateTime dateFrom, DateTime dateTo);
        Task<List<PurchaseDetailLineDTO>> GetDetailsByPurchaseIdAsync(long purchaseId);
        Task<long>                        AddFullPurchaseAsync(Purchase purchase, List<PurchaseDetail> details);
        Task                              ConfirmAsync(long purchaseId, long systemUserId, string paymentMethod);
    }
}
