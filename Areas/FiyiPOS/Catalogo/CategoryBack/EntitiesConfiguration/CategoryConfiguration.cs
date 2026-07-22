using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.CategoryBack.EntitiesConfiguration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            //CategoryId
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryId)
                .ValueGeneratedOnAdd();

            //StoreId
            entity.Property(e => e.StoreId)
                .HasColumnType("bigint")
                .IsRequired(true);

            //Name
            entity.Property(e => e.Name)
                .HasColumnType("varchar(100)")
                .IsRequired(true);

            //Description
            entity.Property(e => e.Description)
                .HasColumnType("varchar(300)")
                .IsRequired(false);

            //Active
            entity.Property(e => e.Active)
                .HasColumnType("tinyint")
                .IsRequired(true);
        }
    }
}
