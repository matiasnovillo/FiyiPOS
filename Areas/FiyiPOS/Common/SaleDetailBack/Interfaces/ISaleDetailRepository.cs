using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Interfaces
{
    public partial interface ISaleDetailRepository
    {
        Task<List<SaleDetail>> GetAllBySaleIdAsync(long saleId);
    }
}
