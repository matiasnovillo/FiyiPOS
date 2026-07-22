using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.System.FailureBack.Entities;

/*
 * 
 * Coded by fiyistack.com
 * Copyright © 2026
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 */

namespace FiyiPOS.Areas.System.FailureBack.EntitiesConfiguration
{
    public class FailureConfiguration : IEntityTypeConfiguration<Failure>
    {
        public void Configure(EntityTypeBuilder<Failure> entity)
        {
            //FailureId
            entity.HasKey(e => e.FailureId);
            entity.Property(e => e.FailureId)
                .ValueGeneratedOnAdd();

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

            //Message
            entity.Property(e => e.Message)
                .HasColumnType("varchar(MAX)")
                .IsRequired(true);

            //EmergencyLevel
            entity.Property(e => e.EmergencyLevel)
                .HasColumnType("bigint")
                .IsRequired(true);

            //StackTrace
            entity.Property(e => e.StackTrace)
                .HasColumnType("varchar(MAX)")
                .IsRequired(true);

            //Source
            entity.Property(e => e.Source)
                .HasColumnType("varchar(MAX)")
                .IsRequired(true);

            //Comment
            entity.Property(e => e.Comment)
                .HasColumnType("varchar(MAX)")
                .IsRequired(true);

            
        }
    }
}