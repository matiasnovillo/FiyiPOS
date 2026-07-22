using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.EntitiesConfiguration
{
    public class AfipAccessTicketConfiguration : IEntityTypeConfiguration<AfipAccessTicket>
    {
        public void Configure(EntityTypeBuilder<AfipAccessTicket> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CompanySettingsId).HasColumnType("int").IsRequired();
            entity.Property(e => e.ServiceName).HasColumnType("varchar(50)").IsRequired();
            entity.Property(e => e.Token).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(e => e.Sign).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(e => e.ExpirationTime).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
            entity.HasIndex(e => new { e.CompanySettingsId, e.ServiceName }).IsUnique();
        }
    }
}
