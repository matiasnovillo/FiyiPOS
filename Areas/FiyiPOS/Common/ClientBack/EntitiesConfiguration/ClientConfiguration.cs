using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.ClientBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.ClientBack.EntitiesConfiguration
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            //ClientId
            entity.HasKey(e => e.ClientId);
            entity.Property(e => e.ClientId)
                .ValueGeneratedOnAdd();

            //StoreId
            entity.Property(e => e.StoreId)
                .HasColumnType("bigint")
                .IsRequired(true);

            //DateTimeCreation
            entity.Property(e => e.DateTimeCreation)
                .HasColumnType("datetime")
                .IsRequired(true);

            //DateTimeLastModification
            entity.Property(e => e.DateTimeLastModification)
                .HasColumnType("datetime")
                .IsRequired(true);

            //UserCreationId
            entity.Property(e => e.UserCreationId)
                .HasColumnType("bigint")
                .IsRequired(true);

            //UserLastModificationId
            entity.Property(e => e.UserLastModificationId)
                .HasColumnType("bigint")
                .IsRequired(true);

            //Active
            entity.Property(e => e.Active)
                .HasColumnType("tinyint")
                .IsRequired(true);

            //Nombres
            entity.Property(e => e.Nombres)
                .HasColumnType("varchar(200)")
                .IsRequired(true);

            //Apellidos
            entity.Property(e => e.Apellidos)
                .HasColumnType("varchar(200)")
                .IsRequired(true);

            //Telefono
            entity.Property(e => e.Telefono)
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            //Correo
            entity.Property(e => e.Correo)
                .HasColumnType("varchar(400)")
                .IsRequired(false);

            //Domicilio
            entity.Property(e => e.Domicilio)
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            //RFC
            entity.Property(e => e.CUIT)
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            //Notas
            entity.Property(e => e.Notas)
                .HasColumnType("varchar(max)")
                .IsRequired(false);

            //TieneCreditoAutorizado
            entity.Property(e => e.TieneCreditoAutorizado)
                .HasColumnType("tinyint")
                .IsRequired(true);

            //LimiteCredito
            entity.Property(e => e.LimiteCredito)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);

            //SaldoCredito
            entity.Property(e => e.SaldoCredito)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);
        }
    }
}
