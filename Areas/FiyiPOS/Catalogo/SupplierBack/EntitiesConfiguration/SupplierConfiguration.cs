using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.SupplierBack.EntitiesConfiguration
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> entity)
        {
            //SupplierId
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.SupplierId)
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

            //Name
            entity.Property(e => e.Name)
                .HasColumnType("varchar(150)")
                .IsRequired(true);

            //ContactName
            entity.Property(e => e.ContactName)
                .HasColumnType("varchar(100)")
                .IsRequired(false);

            //Phone
            entity.Property(e => e.Phone)
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            //Email
            entity.Property(e => e.Email)
                .HasColumnType("varchar(150)")
                .IsRequired(false);

            //Address
            entity.Property(e => e.Address)
                .HasColumnType("varchar(300)")
                .IsRequired(false);

            //TaxId
            entity.Property(e => e.TaxId)
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            //Notes
            entity.Property(e => e.Notes)
                .HasColumnType("varchar(max)")
                .IsRequired(false);
        }
    }
}
