using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseDetailBack.EntitiesConfiguration
{
    public class PurchaseDetailConfiguration : IEntityTypeConfiguration<PurchaseDetail>
    {
        public void Configure(EntityTypeBuilder<PurchaseDetail> entity)
        {
            entity.HasKey(e => e.PurchaseDetailId);
            entity.Property(e => e.PurchaseDetailId).ValueGeneratedOnAdd();

            entity.Property(e => e.PurchaseId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.ProductId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.Quantity).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.UnitCost).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Subtotal).HasColumnType("numeric(18,2)").IsRequired(true);
        }
    }
}
