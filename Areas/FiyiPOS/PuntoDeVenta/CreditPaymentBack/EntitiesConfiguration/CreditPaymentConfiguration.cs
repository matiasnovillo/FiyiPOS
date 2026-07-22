using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.EntitiesConfiguration
{
    public class CreditPaymentConfiguration : IEntityTypeConfiguration<CreditPayment>
    {
        public void Configure(EntityTypeBuilder<CreditPayment> entity)
        {
            entity.HasKey(e => e.CreditPaymentId);
            entity.Property(e => e.CreditPaymentId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreditId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.SystemUserId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.PaymentMethodId).HasColumnType("bigint").IsRequired(true);
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)").IsRequired(true);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime").IsRequired(true);
            entity.Property(e => e.Notes).HasColumnType("varchar(300)").IsRequired(false);
        }
    }
}
