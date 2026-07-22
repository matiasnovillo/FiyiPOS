using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.EntitiesConfiguration
{
    public class CashMovementConfiguration : IEntityTypeConfiguration<CashMovement>
    {
        public void Configure(EntityTypeBuilder<CashMovement> entity)
        {
            entity.HasKey(e => e.CashMovementId);
            entity.Property(e => e.CashMovementId).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.Type).HasColumnType("varchar(3)").IsRequired(true);
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Reason).HasColumnType("varchar(200)").IsRequired(true);
            entity.Property(e => e.MovementDate).HasColumnType("datetime").IsRequired(true);
        }
    }
}
