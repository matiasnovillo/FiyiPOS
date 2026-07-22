using FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.DTOs;

namespace FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.Interfaces
{
    public interface IAttendanceRepository
    {
        Task AddCheckInAsync(long systemUserId, long storeId);
        Task SetCheckOutAsync(long systemUserId, long storeId);
        Task<bool> HasUnclosedSessionAsync(long systemUserId, long storeId);
        Task<List<AttendanceRowDTO>> GetAllByStoreIdsAsync(List<long> storeIds, DateTime from, DateTime to);
    }
}
