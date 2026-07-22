using FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Interfaces
{
    public interface INotepadRepository
    {
        Task<Notepad?> GetByStoreIdAsync(long storeId);
        Task UpdateNoteAsync(long storeId, int noteNum, string content);
    }
}
