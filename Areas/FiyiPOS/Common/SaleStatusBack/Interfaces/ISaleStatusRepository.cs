using FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Interfaces
{
    public partial interface ISaleStatusRepository
    {
        Task<List<SaleStatus>> GetAllAsync();
    }
}
