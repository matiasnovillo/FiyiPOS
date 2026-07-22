namespace FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Entities
{
    public class Notepad
    {
        public long   NotepadId { get; set; }
        public long   StoreId   { get; set; }
        public string Note1     { get; set; } = string.Empty;
        public string Note2     { get; set; } = string.Empty;
        public string Note3     { get; set; } = string.Empty;
        public string Note4     { get; set; } = string.Empty;
    }
}
