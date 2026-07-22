using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.InventoryMovementBack.EntitiesConfiguration
{
    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> entity)
        {
            entity.HasKey(e => e.InventoryMovementId);
            entity.Property(e => e.InventoryMovementId).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.ProductId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SupplierId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.SaleId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.PurchaseId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.MovementType).HasColumnType("varchar(10)").IsRequired(true);
            entity.Property(e => e.Quantity).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.StockBefore).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.StockAfter).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.UnitCost).HasColumnType("numeric(18,2)").IsRequired(false);
            entity.Property(e => e.Reference).HasColumnType("varchar(100)").IsRequired(false);
            entity.Property(e => e.Reason).HasColumnType("varchar(300)").IsRequired(false);
            entity.Property(e => e.MovementDate).HasColumnType("datetime").IsRequired(true);
        }
    }
}
