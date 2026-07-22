using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<long> AddAsync(Invoice invoice);
        Task<Invoice?> GetBySaleIdAsync(long saleId);
        Task<long> GetUltimoNumeroComprobanteAsync(long storeId, string ambiente, int puntoVenta, int tipoComprobante);
        Task<List<InvoiceListItemDTO>> GetAllByStoreIdAsync(long storeId, int take = 200);
    }
}
