using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleDetailBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleDetailBack.EntitiesConfiguration
{
    public class ReturnSaleDetailConfiguration : IEntityTypeConfiguration<ReturnSaleDetail>
    {
        public void Configure(EntityTypeBuilder<ReturnSaleDetail> entity)
        {
            entity.HasKey(e => e.ReturnSaleDetailId);
            entity.Property(e => e.ReturnSaleId).IsRequired();
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.ProductNameSnapshot).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
