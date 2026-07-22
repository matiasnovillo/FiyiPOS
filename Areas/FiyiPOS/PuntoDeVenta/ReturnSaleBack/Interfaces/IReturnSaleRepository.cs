using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Interfaces
{
    public partial interface IReturnSaleRepository
    {
        Task<long> AddFullReturnAsync(ReturnSale header, List<ReturnSaleDetail> details);
        Task<List<ReturnSaleLineDTO>> GetAllByStoreIdAsync(long storeId, int take = 60);
        Task<List<ReturnSaleDetail>> GetDetailsByReturnSaleIdAsync(long returnSaleId);
    }
}
