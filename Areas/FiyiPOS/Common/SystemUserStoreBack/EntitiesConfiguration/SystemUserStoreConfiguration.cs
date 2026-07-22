using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.EntitiesConfiguration
{
    public class SystemUserStoreConfiguration : IEntityTypeConfiguration<SystemUserStore>
    {
        public void Configure(EntityTypeBuilder<SystemUserStore> builder)
        {
            builder.HasKey(x => new { x.SystemUserId, x.StoreId });

            builder.HasOne(x => x.SystemUser)
                .WithMany()
                .HasForeignKey(x => x.SystemUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Store)
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
