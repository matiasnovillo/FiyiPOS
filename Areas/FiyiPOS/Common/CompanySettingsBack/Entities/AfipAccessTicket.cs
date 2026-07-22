namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities
{
    public class AfipAccessTicket
    {
        public int Id { get; set; }
        public int CompanySettingsId { get; set; }
        public string ServiceName { get; set; } = "wsfe";
        public string Token { get; set; } = "";
        public string Sign { get; set; } = "";
        public DateTime ExpirationTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
