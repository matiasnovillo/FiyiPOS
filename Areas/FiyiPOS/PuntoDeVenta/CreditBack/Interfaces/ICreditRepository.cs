using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Interfaces
{
    public interface ICreditRepository
    {
        Task<List<CreditLineDTO>> GetAllByStoreIdAsync(long storeId, bool onlyOpen = true);
        Task<decimal> GetOpenBalanceByClientIdAsync(long clientId);
        Task AddAsync(Credit credit, long storeId);
    }
}
