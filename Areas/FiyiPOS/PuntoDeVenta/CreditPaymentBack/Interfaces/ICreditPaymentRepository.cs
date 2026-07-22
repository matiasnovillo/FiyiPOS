using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Interfaces
{
    public interface ICreditPaymentRepository
    {
        Task<List<CreditPaymentLineDTO>> GetAllByCreditIdAsync(long creditId);
        Task AddAsync(CreditPayment payment);
    }
}
