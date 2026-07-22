using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.EntitiesConfiguration
{
    public class ConfigConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> entity)
        {
            entity.HasKey(e => e.ConfigId);
            entity.Property(e => e.ConfigId).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired(false);
            entity.Property(e => e.ConfigKey).HasColumnName("Key").HasColumnType("varchar(100)").IsRequired(true);
            entity.Property(e => e.Value).HasColumnType("varchar(500)").IsRequired(true);
            entity.Property(e => e.Description).HasColumnType("varchar(300)").IsRequired(false);
        }
    }
}
