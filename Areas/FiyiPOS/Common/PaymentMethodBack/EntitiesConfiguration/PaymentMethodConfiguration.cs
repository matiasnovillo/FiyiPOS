using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.PaymentMethodBack.EntitiesConfiguration
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> entity)
        {
            entity.HasKey(e => e.PaymentMethodId);
            entity.Property(e => e.PaymentMethodId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasColumnType("varchar(50)").IsRequired(true);
            entity.Property(e => e.RequiresRef).HasColumnType("tinyint").IsRequired(true);
            entity.Property(e => e.IsCash).HasColumnType("tinyint").IsRequired(true);
            entity.Property(e => e.Active).HasColumnType("tinyint").IsRequired(true);
        }
    }
}
