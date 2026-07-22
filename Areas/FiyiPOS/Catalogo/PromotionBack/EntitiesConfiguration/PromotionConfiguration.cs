using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PromotionBack.EntitiesConfiguration
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> entity)
        {
            entity.HasKey(e => e.PromotionId);
            entity.Property(e => e.PromotionId).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.Name).HasColumnType("varchar(150)").IsRequired(true);
            entity.Property(e => e.Description).HasColumnType("varchar(500)").IsRequired(false);
            entity.Property(e => e.Price).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Active).HasColumnType("bit").IsRequired(true);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired(true);
        }
    }
}
