using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.EntitiesConfiguration
{
    public class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
    {
        public void Configure(EntityTypeBuilder<CompanySettings> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired();
            entity.Property(e => e.CompanyName).HasColumnType("nvarchar(200)").IsRequired();
            entity.Property(e => e.LegalName).HasColumnType("nvarchar(200)").IsRequired();
            entity.Property(e => e.TaxId).HasColumnType("varchar(11)").IsRequired();
            entity.Property(e => e.Address).HasColumnType("nvarchar(300)").IsRequired();
            entity.Property(e => e.Phone).HasColumnType("nvarchar(50)").IsRequired();
            entity.Property(e => e.TaxCondition).HasColumnType("varchar(50)").IsRequired();
            entity.Property(e => e.PointOfSale).HasColumnType("int").IsRequired();
            entity.Property(e => e.Environment).HasColumnType("varchar(20)").IsRequired();
            entity.Property(e => e.InvoicingEnabled).HasColumnType("bit").IsRequired();
            entity.Property(e => e.CertificatePfx).HasColumnType("varbinary(max)").IsRequired(false);
            entity.Property(e => e.CertificatePassword).HasColumnType("nvarchar(500)").IsRequired(false);
            entity.Property(e => e.CertificateSubject).HasColumnType("nvarchar(500)").IsRequired(false);
            entity.Property(e => e.CertificateIssuer).HasColumnType("nvarchar(500)").IsRequired(false);
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime").IsRequired(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime").IsRequired();
            entity.HasIndex(e => e.StoreId).IsUnique();
        }
    }
}
