using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionDetailBack.EntitiesConfiguration
{
    public class PromotionDetailConfiguration : IEntityTypeConfiguration<PromotionDetail>
    {
        public void Configure(EntityTypeBuilder<PromotionDetail> entity)
        {
            entity.HasKey(e => e.PromotionDetailId);
            entity.Property(e => e.PromotionDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.PromotionId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.ProductId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.ProductNameSnapshot).HasColumnType("varchar(150)").IsRequired(true);
            entity.Property(e => e.Quantity).HasColumnType("numeric(18,2)").IsRequired(true);
        }
    }
}
