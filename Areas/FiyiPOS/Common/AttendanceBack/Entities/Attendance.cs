namespace FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.Entities
{
    public class Attendance
    {
        public long      AttendanceId { get; set; }
        public long      SystemUserId { get; set; }
        public long      StoreId      { get; set; }
        public DateTime  CheckIn      { get; set; }
        public DateTime? CheckOut     { get; set; }   // null = still active
    }
}
