using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Interfaces;
using FiyiPOS.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace FiyiPOS.Areas.FiyiPOS.Common.InvoiceBack.Repositories
{
    public class InvoiceRepository(IDbContextFactory<FiyiPOSDbContext> factory) : IInvoiceRepository
    {
        public async Task<long> AddAsync(Invoice invoice)
        {
            await using var db = await factory.CreateDbContextAsync();
            invoice.CreatedAt = DateTime.Now;
            invoice.InvoiceDate = DateTime.Now;
            await db.Invoice.AddAsync(invoice);
            await db.SaveChangesAsync();
            return invoice.InvoiceId;
        }

        public async Task<Invoice?> GetBySaleIdAsync(long saleId)
        {
            await using var db = await factory.CreateDbContextAsync();
            return await db.Invoice.AsNoTracking()
                .FirstOrDefaultAsync(i => i.SaleId == saleId);
        }

        public async Task<long> GetUltimoNumeroComprobanteAsync(
            long storeId, string ambiente, int puntoVenta, int tipoComprobante)
        {
            await using var db = await factory.CreateDbContextAsync();
            return await db.Invoice
                .Where(i => i.StoreId == storeId
                         && i.Environment == ambiente
                         && i.PointOfSale == puntoVenta
                         && i.DocumentType == tipoComprobante)
                .MaxAsync(i => (long?)i.DocumentNumber) ?? 0L;
        }

        public async Task<List<InvoiceListItemDTO>> GetAllByStoreIdAsync(long storeId, int take = 200)
        {
            await using var db = await factory.CreateDbContextAsync();
            return await (
                from i in db.Invoice
                join s in db.Sale on i.SaleId equals s.SaleId into sj
                from s in sj.DefaultIfEmpty()
                join c in db.Client on i.ClientId equals c.ClientId into cj
                from c in cj.DefaultIfEmpty()
                where i.StoreId == storeId
                orderby i.InvoiceDate descending
                select new InvoiceListItemDTO
                {
                    InvoiceId           = i.InvoiceId,
                    SaleId              = i.SaleId,
                    InvoiceNumber       = i.InvoiceNumber,
                    TipoComprobante     = i.DocumentType,
                    PuntoVenta          = i.PointOfSale,
                    NumeroComprobante   = i.DocumentNumber,
                    CAE                 = i.CAE,
                    CAEFechaVencimiento = i.CAEExpirationDate,
                    InvoiceDate         = i.InvoiceDate,
                    Subtotal            = i.Subtotal,
                    TaxAmount           = i.TaxAmount,
                    Total               = i.Total,
                    Status              = i.Status,
                    AfipResultado       = i.AfipResult,
                    Ambiente            = i.Environment,
                    TicketNumber        = s != null ? s.TicketNumber : null,
                    ClientName          = c != null ? (c.Nombres + " " + c.Apellidos) : null
                }
            ).Take(take).ToListAsync();
        }
    }
}
