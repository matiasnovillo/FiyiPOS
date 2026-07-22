using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.PurchaseBack.EntitiesConfiguration
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> entity)
        {
            entity.HasKey(e => e.PurchaseId);
            entity.Property(e => e.PurchaseId).ValueGeneratedOnAdd();

            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SupplierId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SupplierInvoice).HasColumnType("varchar(50)").IsRequired(false);
            entity.Property(e => e.PurchaseDate).HasColumnType("datetime").IsRequired(true);
            entity.Property(e => e.Subtotal).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.TaxAmount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Total).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Notes).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.IsConfirmed).HasColumnType("tinyint").IsRequired(true);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired(true);
        }
    }
}
