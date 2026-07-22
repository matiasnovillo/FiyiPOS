using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Interfaces
{
    public partial interface IInternalConsumptionRepository
    {
        Task<long> AddFullConsumptionAsync(InternalConsumption header, List<InternalConsumptionDetail> details);
        Task<List<InternalConsumptionLineDTO>> GetAllByStoreIdAsync(long storeId, int take = 200);
        Task<List<InternalConsumptionDetail>> GetDetailsByInternalConsumptionIdAsync(long internalConsumptionId);
    }
}
