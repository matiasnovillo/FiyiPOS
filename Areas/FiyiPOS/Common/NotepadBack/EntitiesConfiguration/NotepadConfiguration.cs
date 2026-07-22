using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.NotepadBack.EntitiesConfiguration
{
    public class NotepadConfiguration : IEntityTypeConfiguration<Notepad>
    {
        public void Configure(EntityTypeBuilder<Notepad> entity)
        {
            entity.HasKey(e => e.NotepadId);
            entity.Property(e => e.NotepadId).ValueGeneratedOnAdd();

            entity.Property(e => e.StoreId)
                .HasColumnType("bigint")
                .IsRequired(true);

            entity.HasIndex(e => e.StoreId).IsUnique();

            entity.Property(e => e.Note1).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.Note2).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.Note3).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.Note4).HasColumnType("varchar(max)").IsRequired(false);
        }
    }
}
