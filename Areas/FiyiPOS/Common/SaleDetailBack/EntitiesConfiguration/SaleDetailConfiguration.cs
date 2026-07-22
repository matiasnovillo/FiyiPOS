using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleDetailBack.EntitiesConfiguration
{
    public class SaleDetailConfiguration : IEntityTypeConfiguration<SaleDetail>
    {
        public void Configure(EntityTypeBuilder<SaleDetail> entity)
        {
            entity.HasKey(e => e.SaleDetailId);
            entity.Property(e => e.SaleDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.SaleId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.ProductId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.BarcodeSnapshot).HasColumnType("varchar(50)").IsRequired(false);
            entity.Property(e => e.ProductNameSnapshot).HasColumnType("varchar(150)").IsRequired(true);
            entity.Property(e => e.Quantity).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.UnitPrice).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Discount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.TaxRate).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Subtotal).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.CostPriceSnapshot).HasColumnType("numeric(18,2)").IsRequired(false);
            entity.Property(e => e.PromotionId).HasColumnType("bigint").IsRequired(false);
        }
    }
}
