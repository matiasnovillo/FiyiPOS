using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Interfaces;
using FiyiPOS.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace FiyiPOS.Areas.FiyiPOS.Common.CompanySettingsBack.Repositories
{
    public class CompanySettingsRepository(IDbContextFactory<FiyiPOSDbContext> factory) : ICompanySettingsRepository
    {
        public async Task<CompanySettings?> GetByStoreIdAsync(long storeId)
        {
            await using var db = await factory.CreateDbContextAsync();
            return await db.CompanySettings.AsNoTracking()
                .FirstOrDefaultAsync(e => e.StoreId == storeId);
        }

        public async Task<CompanySettings?> GetByStoreIdForUpdateAsync(long storeId)
        {
            await using var db = await factory.CreateDbContextAsync();
            return await db.CompanySettings
                .FirstOrDefaultAsync(e => e.StoreId == storeId);
        }

        public async Task<CompanySettings> UpsertAsync(CompanySettings config)
        {
            await using var db = await factory.CreateDbContextAsync();
            var existing = await db.CompanySettings
                .FirstOrDefaultAsync(e => e.StoreId == config.StoreId);

            if (existing is null)
            {
                config.CreatedAt = DateTime.Now;
                config.ModifiedAt = DateTime.Now;
                await db.CompanySettings.AddAsync(config);
            }
            else
            {
                existing.CompanyName = config.CompanyName;
                existing.LegalName = config.LegalName;
                existing.TaxId = config.TaxId;
                existing.Address = config.Address;
                existing.Phone = config.Phone;
                existing.TaxCondition = config.TaxCondition;
                existing.PointOfSale = config.PointOfSale;
                existing.Environment = config.Environment;
                existing.InvoicingEnabled = config.InvoicingEnabled;
                if (config.CertificatePfx is not null)
                    existing.CertificatePfx = config.CertificatePfx;
                if (!string.IsNullOrEmpty(config.CertificatePassword))
                    existing.CertificatePassword = config.CertificatePassword;
                if (!string.IsNullOrEmpty(config.CertificateSubject))
                    existing.CertificateSubject = config.CertificateSubject;
                if (!string.IsNullOrEmpty(config.CertificateIssuer))
                    existing.CertificateIssuer = config.CertificateIssuer;
                if (config.ExpirationDate.HasValue)
                    existing.ExpirationDate = config.ExpirationDate;
                existing.ModifiedAt = DateTime.Now;
                config = existing;
            }

            await db.SaveChangesAsync();
            return config;
        }
    }
}
