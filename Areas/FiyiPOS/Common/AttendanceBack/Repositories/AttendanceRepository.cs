using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.Repositories
{
    public partial class AttendanceRepository(
        IDbContextFactory<FiyiPOSDbContext> _factory) : IAttendanceRepository
    {
        public async Task AddCheckInAsync(long systemUserId, long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await db.Attendance.AddAsync(new Attendance
            {
                SystemUserId = systemUserId,
                StoreId      = storeId,
                CheckIn      = DateTime.Now,
            });
            await db.SaveChangesAsync();
        }

        public async Task SetCheckOutAsync(long systemUserId, long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var records = await db.Attendance
                .Where(a => a.SystemUserId == systemUserId &&
                            a.StoreId      == storeId      &&
                            a.CheckOut     == null)
                .ToListAsync();

            if (records.Count == 0) return;

            var now = DateTime.Now;
            foreach (var r in records)
                r.CheckOut = now;

            await db.SaveChangesAsync();
        }

        public async Task<bool> HasUnclosedSessionAsync(long systemUserId, long storeId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Attendance
                .AsNoTracking()
                .AnyAsync(a => a.SystemUserId == systemUserId &&
                               a.StoreId      == storeId      &&
                               a.CheckOut     == null);
        }

        public async Task<List<AttendanceRowDTO>> GetAllByStoreIdsAsync(
            List<long> storeIds, DateTime from, DateTime to)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Attendance
                .AsNoTracking()
                .Where(a => storeIds.Contains(a.StoreId) &&
                            a.CheckIn >= from && a.CheckIn < to)
                .Join(db.SystemUser,
                      a => a.SystemUserId,
                      u => u.SystemUserId,
                      (a, u) => new { a, u.Name })
                .Join(db.Store,
                      x => x.a.StoreId,
                      s => s.StoreId,
                      (x, s) => new AttendanceRowDTO
                      {
                          AttendanceId = x.a.AttendanceId,
                          OperatorName = x.Name,
                          StoreName    = s.Name,
                          CheckIn      = x.a.CheckIn,
                          CheckOut     = x.a.CheckOut,
                      })
                .OrderByDescending(x => x.CheckIn)
                .ToListAsync();
        }
    }
}
