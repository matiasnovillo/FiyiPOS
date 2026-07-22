using FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiyiPOS.Areas.FiyiPOS.Common.AfipLogBack.EntitiesConfiguration
{
    public class AfipLogConfiguration : IEntityTypeConfiguration<AfipLog>
    {
        public void Configure(EntityTypeBuilder<AfipLog> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.StoreId).HasColumnType("bigint").IsRequired();
            entity.Property(e => e.Operation).HasColumnType("varchar(100)").IsRequired();
            entity.Property(e => e.Request).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.Response).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.IsError).HasColumnType("bit").IsRequired();
            entity.Property(e => e.ErrorMessage).HasColumnType("varchar(max)").IsRequired(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
        }
    }
}
