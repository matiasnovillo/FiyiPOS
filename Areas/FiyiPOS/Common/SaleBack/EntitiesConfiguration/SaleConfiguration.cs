using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.SaleBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleBack.EntitiesConfiguration
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> entity)
        {
            entity.HasKey(e => e.SaleId);
            entity.Property(e => e.SaleId).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.ClientId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.SaleStatusId).HasColumnType("int").IsRequired(true);
            entity.Property(e => e.PriceListId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.TicketNumber).HasColumnType("varchar(30)").IsRequired(true);
            entity.HasIndex(e => e.TicketNumber).IsUnique();
            entity.Property(e => e.SaleDate).HasColumnType("datetime").IsRequired(true);
            entity.Property(e => e.Subtotal).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Discount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.TaxAmount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Total).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.AmountPaid).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Change).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Notes).HasColumnType("varchar(500)").IsRequired(false);
            entity.Property(e => e.IsPrinted).HasColumnType("tinyint").IsRequired(true);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired(true);
        }
    }
}
