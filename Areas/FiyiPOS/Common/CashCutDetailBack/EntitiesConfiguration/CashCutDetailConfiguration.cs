using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashCutDetailBack.EntitiesConfiguration
{
    public class CashCutDetailConfiguration : IEntityTypeConfiguration<CashCutDetail>
    {
        public void Configure(EntityTypeBuilder<CashCutDetail> entity)
        {
            entity.HasKey(e => e.CashCutDetailId);
            entity.Property(e => e.CashCutDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.CashCutId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.PaymentMethodId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.TotalCollected).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.TransactionCount).HasColumnType("int").IsRequired(true);
        }
    }
}
