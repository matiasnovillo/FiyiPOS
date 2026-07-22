using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.EntitiesConfiguration
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> entity)
        {
            entity.HasKey(e => e.InvoiceId);
            entity.Property(e => e.InvoiceId).ValueGeneratedOnAdd();
            entity.Property(e => e.SaleId).HasColumnType("bigint").IsRequired();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired();
            entity.Property(e => e.ClientId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired();
            entity.Property(e => e.InvoiceNumber).HasColumnType("varchar(50)").IsRequired();
            entity.Property(e => e.DocumentType).HasColumnType("int").IsRequired();
            entity.Property(e => e.PointOfSale).HasColumnType("int").IsRequired();
            entity.Property(e => e.DocumentNumber).HasColumnType("bigint").IsRequired();
            entity.Property(e => e.CAE).HasColumnType("varchar(14)").IsRequired(false);
            entity.Property(e => e.CAEExpirationDate).HasColumnType("datetime").IsRequired(false);
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Subtotal).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(e => e.TaxAmount).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(e => e.Total).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(e => e.Status).HasColumnType("varchar(20)").IsRequired();
            entity.Property(e => e.AfipResult).HasColumnType("varchar(1)").IsRequired(false);
            entity.Property(e => e.AfipErrors).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.Environment).HasColumnType("varchar(20)").IsRequired();
            entity.Property(e => e.XmlRequest).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.XmlResponse).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
            entity.HasIndex(e => new { e.PointOfSale, e.DocumentType, e.DocumentNumber, e.Environment }).IsUnique();
        }
    }
}
