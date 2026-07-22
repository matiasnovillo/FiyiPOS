using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.EntitiesConfiguration
{
    public class CashCutConfiguration : IEntityTypeConfiguration<CashCut>
    {
        public void Configure(EntityTypeBuilder<CashCut> entity)
        {
            entity.HasKey(e => e.CashCutId);
            entity.Property(e => e.CashCutId).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.OpeningAmount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.ExpectedAmount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.ActualAmount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Difference).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.TotalSales).HasColumnType("int").IsRequired(true);
            entity.Property(e => e.OpenedAt).HasColumnType("datetime").IsRequired(true);
            entity.Property(e => e.ClosedAt).HasColumnType("datetime").IsRequired(false);
            entity.Property(e => e.Notes).HasColumnType("varchar(500)").IsRequired(false);
        }
    }
}
