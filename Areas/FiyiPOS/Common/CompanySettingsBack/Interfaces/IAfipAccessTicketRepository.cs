using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Interfaces
{
    public interface IAfipAccessTicketRepository
    {
        Task<AfipAccessTicket?> GetValidTicketAsync(int companySettingsId, string serviceName);
        Task SaveTicketAsync(AfipAccessTicket ticket);
    }
}
