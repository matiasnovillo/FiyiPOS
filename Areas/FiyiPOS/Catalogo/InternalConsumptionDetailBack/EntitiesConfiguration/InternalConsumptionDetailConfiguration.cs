using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InternalConsumptionDetailBack.EntitiesConfiguration
{
    public class InternalConsumptionDetailConfiguration : IEntityTypeConfiguration<InternalConsumptionDetail>
    {
        public void Configure(EntityTypeBuilder<InternalConsumptionDetail> entity)
        {
            entity.HasKey(e => e.InternalConsumptionDetailId);
            entity.Property(e => e.InternalConsumptionId).IsRequired();
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.ProductNameSnapshot).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
