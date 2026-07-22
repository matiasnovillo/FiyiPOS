using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Interfaces
{
    public partial interface ISalePaymentRepository
    {
        Task<List<SalePayment>> GetAllBySaleIdAsync(long saleId);
    }
}
