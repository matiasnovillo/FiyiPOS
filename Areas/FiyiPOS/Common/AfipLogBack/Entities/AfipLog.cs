namespace FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Entities
{
    public class AfipLog
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public string Operation { get; set; } = "";
        public string? Request { get; set; }
        public string? Response { get; set; }
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
