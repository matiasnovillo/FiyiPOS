namespace FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.DTOs
{
    public class AttendanceRowDTO
    {
        public long      AttendanceId  { get; set; }
        public string    OperatorName  { get; set; } = "";
        public string    StoreName     { get; set; } = "";
        public DateTime  CheckIn       { get; set; }
        public DateTime? CheckOut      { get; set; }

        public bool IsActive => CheckOut is null;

        public string Duration
        {
            get
            {
                TimeSpan span = (CheckOut ?? DateTime.Now) - CheckIn;
                return $"{(int)span.TotalHours:D2}:{span.Minutes:D2}h";
            }
        }
    }
}
