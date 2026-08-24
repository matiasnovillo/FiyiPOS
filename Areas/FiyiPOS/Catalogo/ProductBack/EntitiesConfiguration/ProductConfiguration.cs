using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Catalogo.ProductBack.EntitiesConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            //ProductId
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd();

            //StoreId
            entity.Property(e => e.StoreId)
                .HasColumnType("bigint")
                .IsRequired(true);

            //CategoryId
            entity.Property(e => e.CategoryId)
                .HasColumnType("bigint")
                .IsRequired(false);

            //UnitId
            entity.Property(e => e.UnitId)
                .HasColumnType("bigint")
                .IsRequired(false);

            //Barcode
            entity.Property(e => e.Barcode)
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            //Name
            entity.Property(e => e.Name)
                .HasColumnType("varchar(150)")
                .IsRequired(true);

            //Description
            entity.Property(e => e.Description)
                .HasColumnType("varchar(max)")
                .IsRequired(false);

            //SalePrice
            entity.Property(e => e.SalePrice)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);

            //NightSalePrice
            entity.Property(e => e.NightSalePrice)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);

            //CostPrice
            entity.Property(e => e.CostPrice)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);

            //WholesalePrice
            entity.Property(e => e.WholesalePrice)
                .HasColumnType("numeric(18,2)")
                .IsRequired(false);

            //Stock
            entity.Property(e => e.Stock)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);

            //MinStock
            entity.Property(e => e.MinStock)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);

            //MaxStock
            entity.Property(e => e.MaxStock)
                .HasColumnType("numeric(18,2)")
                .IsRequired(false);

            //TaxRate
            entity.Property(e => e.TaxRate)
                .HasColumnType("numeric(18,2)")
                .IsRequired(true);

            //AllowFractional
            entity.Property(e => e.AllowFractional)
                .HasColumnType("tinyint")
                .IsRequired(true);

            //Active
            entity.Property(e => e.Active)
                .HasColumnType("tinyint")
                .IsRequired(true);

            //IsAvailableToSellOnline
            entity.Property(e => e.IsAvailableToSellOnline)
                .HasColumnType("tinyint")
                .IsRequired(true);

            //FirstImageURL
            entity.Property(e => e.FirstImageURL)
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            //CreatedAt
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired(true);

            //UpdatedAt
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .IsRequired(false);
        }
    }
}
