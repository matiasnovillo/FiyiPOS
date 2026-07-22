using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditBack.EntitiesConfiguration
{
    public class CreditConfiguration : IEntityTypeConfiguration<Credit>
    {
        public void Configure(EntityTypeBuilder<Credit> entity)
        {
            entity.HasKey(e => e.CreditId);
            entity.Property(e => e.CreditId).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.ClientId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SaleId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.OriginalAmount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Balance).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.DueDate).HasColumnType("datetime").IsRequired(false);
            entity.Property(e => e.Notes).HasColumnType("varchar(500)").IsRequired(false);
            entity.Property(e => e.Active).HasColumnType("tinyint").IsRequired(true);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired(true);
        }
    }
}
