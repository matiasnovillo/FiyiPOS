using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Entities;

/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.EntitiesConfiguration
{
    public class SystemUserConfiguration : IEntityTypeConfiguration<SystemUser>
    {
        public void Configure(EntityTypeBuilder<SystemUser> entity)
        {
            entity.HasKey(e => e.SystemUserId);
            entity.Property(e => e.SystemUserId).ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnType("varchar(100)")
                .IsRequired(true);

            entity.Property(e => e.Username)
                .HasColumnType("varchar(200)")
                .IsRequired(true);

            entity.Property(e => e.PasswordHash)
                .HasColumnType("varchar(255)")
                .IsRequired(true);

            entity.Property(e => e.Role)
                .HasColumnType("varchar(20)")
                .IsRequired(true);

            entity.Property(e => e.Active)
                .HasColumnType("tinyint")
                .IsRequired(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired(true);

            entity.Property(e => e.ActivationToken)
                .HasColumnType("varchar(50)")
                .IsRequired(false);
        }
    }
}
