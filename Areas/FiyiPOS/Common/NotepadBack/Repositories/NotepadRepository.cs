using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Repositories
{
    public class NotepadRepository(IDbContextFactory<FiyiPOSDbContext> _factory) : INotepadRepository
    {
        public async Task<Notepad?> GetByStoreIdAsync(long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Notepad
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.StoreId == storeId);
        }

        public async Task UpdateNoteAsync(long storeId, int noteNum, string content)
        {
            await using var db = await _factory.CreateDbContextAsync();

            if (!await db.Notepad.AnyAsync(n => n.StoreId == storeId))
            {
                db.Notepad.Add(new Notepad { StoreId = storeId });
                await db.SaveChangesAsync();
            }

            await (noteNum switch
            {
                1 => db.Notepad.Where(n => n.StoreId == storeId)
                        .ExecuteUpdateAsync(s => s.SetProperty(n => n.Note1, content)),
                2 => db.Notepad.Where(n => n.StoreId == storeId)
                        .ExecuteUpdateAsync(s => s.SetProperty(n => n.Note2, content)),
                3 => db.Notepad.Where(n => n.StoreId == storeId)
                        .ExecuteUpdateAsync(s => s.SetProperty(n => n.Note3, content)),
                _ => db.Notepad.Where(n => n.StoreId == storeId)
                        .ExecuteUpdateAsync(s => s.SetProperty(n => n.Note4, content)),
            });
        }
    }
}
