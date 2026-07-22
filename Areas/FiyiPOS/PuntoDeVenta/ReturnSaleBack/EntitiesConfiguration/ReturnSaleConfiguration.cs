using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.ReturnSaleBack.EntitiesConfiguration
{
    public class ReturnSaleConfiguration : IEntityTypeConfiguration<ReturnSale>
    {
        public void Configure(EntityTypeBuilder<ReturnSale> entity)
        {
            entity.HasKey(e => e.ReturnSaleId);
            entity.Property(e => e.SaleId).IsRequired();
            entity.Property(e => e.StoreId).IsRequired();
            entity.Property(e => e.SystemUserId).IsRequired();
            entity.Property(e => e.ReturnDate).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Reason).HasMaxLength(500).IsRequired(false);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
        }
    }
}
