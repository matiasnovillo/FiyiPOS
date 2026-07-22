using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Interfaces
{
    public partial interface ISaleRepository
    {
        Task<long> AddFullSaleAsync(Sale sale, List<SaleDetail> details, List<SalePayment> payments, List<(long ProductId, decimal Quantity)>? comboStockLines = null);
        Task<Sale?> GetOneBySaleIdAsync(long saleId);
        Task<List<Sale>> GetAllByStoreIdAsync(long storeId, int take = 50);
        Task<SalesSummaryReport> GetSalesSummaryAsync(long storeId, DateTime from, DateTime to);
        Task<List<TopProductLine>> GetTopProductsAsync(long storeId, DateTime from, DateTime to, int top = 20);
        Task<List<OperatorSalesLine>> GetSalesByOperatorAsync(long storeId, DateTime from, DateTime to);
        Task<Sale?> GetOneByTicketNumberAsync(long storeId, string ticketNumber);
        Task<List<SaleDetail>> GetDetailsBySaleIdAsync(long saleId);
        Task<List<SaleListLine>> GetSalesByDateRangeAsync(long storeId, DateTime from, DateTime to);
        Task<string> GetPaymentMethodNameBySaleIdAsync(long saleId);
        Task<List<ProductCategoryGroup>> GetProductsByCategoryAsync(long storeId, DateTime from, DateTime to);
    }
}
