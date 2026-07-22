using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Interfaces
{
    public partial interface ICashCutRepository
    {
        Task<CashCutPreviewDTO>  GetPreviewAsync(long storeId);
        Task<List<CashCut>>     GetAllByStoreIdAsync(long storeId, int take = 30);
        Task<long>              CreateAsync(CashCut cashCut, List<CashCutDetail> details);
        Task<CashCutReportDTO>  GetReportDataAsync(long cashCutId, string storeName);
        Task<(List<CashCut> Items, int TotalCount, int TotalSales, int ShortfallCount, int SurplusCount)>
            GetPagedAsync(long storeId, int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo, string? filter);
        Task<int> GetUncountedSalesCountAsync(long storeId);
    }
}
