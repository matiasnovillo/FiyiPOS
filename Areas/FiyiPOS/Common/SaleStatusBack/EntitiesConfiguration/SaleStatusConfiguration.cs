using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SaleStatusBack.EntitiesConfiguration
{
    public class SaleStatusConfiguration : IEntityTypeConfiguration<SaleStatus>
    {
        public void Configure(EntityTypeBuilder<SaleStatus> entity)
        {
            entity.HasKey(e => e.SaleStatusId);
            entity.Property(e => e.SaleStatusId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasColumnType("varchar(50)").IsRequired(true);
        }
    }
}
