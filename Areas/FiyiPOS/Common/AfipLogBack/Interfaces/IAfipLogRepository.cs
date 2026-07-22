using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Interfaces
{
    public interface IAfipLogRepository
    {
        Task AddAsync(AfipLog log);
    }
}
