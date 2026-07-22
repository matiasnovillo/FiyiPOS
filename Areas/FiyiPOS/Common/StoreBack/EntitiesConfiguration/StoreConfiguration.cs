using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.StoreBack.EntitiesConfiguration
{
    public class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> entity)
        {
            entity.HasKey(e => e.StoreId);
            entity.Property(e => e.StoreId).ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnType("varchar(150)")
                .IsRequired(true);

            entity.Property(e => e.Address)
                .HasColumnType("varchar(300)")
                .IsRequired(false);

            entity.Property(e => e.Phone)
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            entity.Property(e => e.TaxId)
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            entity.Property(e => e.LogoUrl)
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            entity.Property(e => e.Active)
                .HasColumnType("tinyint")
                .IsRequired(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired(true);

            entity.Property(e => e.BillingDueDate)
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.ClaudeApiKey)
                .HasColumnType("varchar(500)")
                .IsRequired(false);
        }
    }
}
