using FiyiPOS.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace FiyiPOS.Services.FiyiPOS
{
    public class DbHealthCheckService(
        IDbContextFactory<FiyiPOSDbContext> _factory,
        SystemStatusService _statusService) : BackgroundService
    {
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(15);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait a moment after startup before the first check
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAsync();
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CheckAsync()
        {
            try
            {
                await using var db = await _factory.CreateDbContextAsync();
                bool canConnect = await db.Database.CanConnectAsync();
                if (canConnect) _statusService.SetOnline();
                else            _statusService.SetOffline();
            }
            catch
            {
                _statusService.SetOffline();
            }
        }
    }
}
