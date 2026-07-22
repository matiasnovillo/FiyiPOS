using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.AttendanceBack.EntitiesConfiguration
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> entity)
        {
            entity.HasKey(e => e.AttendanceId);
            entity.Property(e => e.AttendanceId).ValueGeneratedOnAdd();
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.CheckIn).HasColumnType("datetime").IsRequired(true);
            entity.Property(e => e.CheckOut).HasColumnType("datetime").IsRequired(false);
        }
    }
}
