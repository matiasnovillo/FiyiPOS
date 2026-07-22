namespace FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Entities
{
    public class Config
    {
        public int     ConfigId    { get; set; }
        public long?   StoreId     { get; set; }
        public string  ConfigKey   { get; set; } = "";   // maps to [Key] column
        public string  Value       { get; set; } = "";
        public string? Description { get; set; }
    }
}
