using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionBack.EntitiesConfiguration
{
    public class InternalConsumptionConfiguration : IEntityTypeConfiguration<InternalConsumption>
    {
        public void Configure(EntityTypeBuilder<InternalConsumption> entity)
        {
            entity.HasKey(e => e.InternalConsumptionId);
            entity.Property(e => e.StoreId).IsRequired();
            entity.Property(e => e.SystemUserId).IsRequired();
            entity.Property(e => e.ConsumptionDate).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Reason).HasMaxLength(300).IsRequired();
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
        }
    }
}
