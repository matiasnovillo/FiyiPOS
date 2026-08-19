using Microsoft.EntityFrameworkCore;
using FiyiPOS.Areas.FiyiPOS.Common.CashMovementBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.DTOs;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Entities;
using FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Interfaces;
using FiyiPOS.DatabaseContexts;

namespace FiyiPOS.Areas.FiyiPOS.PuntoDeVenta.CreditPaymentBack.Repositories
{
    public class CreditPaymentRepository(IDbContextFactory<FiyiPOSDbContext> _factory) : ICreditPaymentRepository
    {
        public async Task<List<CreditPaymentLineDTO>> GetAllByCreditIdAsync(long creditId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await (
                from cp in db.CreditPayment
                join pm in db.PaymentMethod on cp.PaymentMethodId equals pm.PaymentMethodId
                where cp.CreditId == creditId
                orderby cp.PaymentDate descending
                select new CreditPaymentLineDTO
                {
                    CreditPaymentId   = cp.CreditPaymentId,
                    PaymentMethodName = pm.Name,
                    Amount            = cp.Amount,
                    PaymentDate       = cp.PaymentDate,
                    Notes             = cp.Notes,
                }
            ).ToListAsync();
        }

        // Saves the payment and reduces Credit.Balance atomically.
        // Closes the credit (Active=false) when Balance reaches zero.
        public async Task AddAsync(CreditPayment payment)
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                var credit = await db.Credit.FirstOrDefaultAsync(c => c.CreditId == payment.CreditId)
                    ?? throw new InvalidOperationException("Crédito no encontrado.");

                if (!credit.Active)
                    throw new InvalidOperationException("Este crédito ya está saldado.");

                if (payment.Amount <= 0)
                    throw new ArgumentException("El monto del abono debe ser mayor a cero.");

                if (payment.Amount > credit.Balance)
                    throw new InvalidOperationException(
                        $"El abono (${payment.Amount:N2}) supera el saldo pendiente (${credit.Balance:N2}).");

                decimal newBalance = credit.Balance - payment.Amount;

                await db.Credit
                    .Where(c => c.CreditId == payment.CreditId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(c => c.Balance, newBalance)
                        .SetProperty(c => c.Active,  newBalance > 0));

                payment.PaymentDate = DateTime.Now;
                await db.CreditPayment.AddAsync(payment);

                var pm = await db.PaymentMethod.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PaymentMethodId == payment.PaymentMethodId);

                if (pm?.IsCash == true)
                {
                    var client = await db.Client.AsNoTracking()
                        .FirstOrDefaultAsync(c => c.ClientId == credit.ClientId);
                    string clientName = client != null
                        ? $"{client.Nombres} {client.Apellidos}".Trim()
                        : $"cliente #{credit.ClientId}";

                    await db.CashMovement.AddAsync(new CashMovement
                    {
                        StoreId         = credit.StoreId,
                        SystemUserId    = payment.SystemUserId,
                        PaymentMethodId = pm.PaymentMethodId,
                        Type            = "IN",
                        Amount          = payment.Amount,
                        Reason          = $"Abono crédito #{payment.CreditId} — {clientName}",
                        MovementDate    = payment.PaymentDate,
                    });
                }

                await db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
