using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.UnitBack.EntitiesConfiguration
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> entity)
        {
            //UnitId
            entity.HasKey(e => e.UnitId);
            entity.Property(e => e.UnitId)
                .ValueGeneratedOnAdd();

            //StoreId
            entity.Property(e => e.StoreId)
                .HasColumnType("bigint")
                .IsRequired(true);

            //Name
            entity.Property(e => e.Name)
                .HasColumnType("varchar(50)")
                .IsRequired(true);

            //Abbreviation
            entity.Property(e => e.Abbreviation)
                .HasColumnType("varchar(10)")
                .IsRequired(true);

            //Active
            entity.Property(e => e.Active)
                .HasColumnType("tinyint")
                .IsRequired(true);
        }
    }
}
