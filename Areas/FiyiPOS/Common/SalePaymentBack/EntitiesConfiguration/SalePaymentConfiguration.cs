using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SalePaymentBack.EntitiesConfiguration
{
    public class SalePaymentConfiguration : IEntityTypeConfiguration<SalePayment>
    {
        public void Configure(EntityTypeBuilder<SalePayment> entity)
        {
            entity.HasKey(e => e.SalePaymentId);
            entity.Property(e => e.SalePaymentId).ValueGeneratedOnAdd();
            entity.Property(e => e.SaleId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.PaymentMethodId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.Reference).HasColumnType("varchar(100)").IsRequired(false);
        }
    }
}
