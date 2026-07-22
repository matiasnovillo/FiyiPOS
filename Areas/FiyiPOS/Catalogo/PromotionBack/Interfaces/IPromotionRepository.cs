using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Interfaces
{
    public partial interface IPromotionRepository
    {
        Task<List<PromotionVM>> GetAllActiveByStoreIdAsync(long storeId);
        Task<List<PromotionVM>> GetAllByStoreIdAsync(long storeId);
        Task<long> SaveAsync(Promotion promotion, List<PromotionDetail> details);
        Task DeleteAsync(long promotionId);
    }
}
