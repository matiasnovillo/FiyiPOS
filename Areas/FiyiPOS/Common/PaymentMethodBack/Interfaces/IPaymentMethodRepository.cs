using FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Interfaces
{
    public partial interface IPaymentMethodRepository
    {
        Task<List<PaymentMethod>> GetAllAsync();
        Task<List<PaymentMethod>> GetAllActiveAsync();
    }
}
