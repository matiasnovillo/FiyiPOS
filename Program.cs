using System.Threading.RateLimiting;
using FiyiPOS.Components;
using FiyiPOS.DatabaseContexts;
using FiyiPOS.Mailing;
using FiyiPOS.Services.AFIP;
using FiyiPOS.Services.FiyiPOS;
using FiyiPOS.Services.System;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

//////////////////////////////////////////////////////////////////////////////////////////////////////

var builder = WebApplication.CreateBuilder(args);

#region BUILDER.SERVICES.ADD...
//ADD SERVICES FOR CONTROLLERS
builder.Services.AddControllers();

//ADD SERVICE FOR RAZOR COMPONENTS
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//ADD SERVICE TO USE DATABASE (SCOPED)
builder.Services.AddDbContextFactory<FiyiPOSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//SCRUTOR
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type =>
        type.Name.EndsWith("Repository") ||
        type.Name.EndsWith("ImportationService") ||
        type.Name.EndsWith("ExportationService")))
    .AsSelf()
    .WithScopedLifetime());

//ADD SCOPED SERVICE TO SEND EMAILS
builder.Services.AddScoped<Email>();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<PosSessionService>();
builder.Services.AddScoped<HotkeyService>();
builder.Services.AddSingleton<SystemStatusService>();
builder.Services.AddHostedService<DbHealthCheckService>();
builder.Services.AddScoped<AfipCertificateProtectionService>();
builder.Services.AddScoped<AfipWsaaClient>();
builder.Services.AddScoped<AfipWsfeClient>();
builder.Services.AddScoped<AfipQrService>();
builder.Services.AddScoped<AfipTokenService>();
builder.Services.AddScoped<AfipFacturaElectronicaService>();

// RATE LIMITING — 300 requests/min por IP; exceso devuelve HTTP 429
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit          = 300,
                Window               = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit           = 0
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
#endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/500", createScopeForErrors: true);
    //MORE INFO IN -> https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#region APP.USE...
app.UseRateLimiter();

app.UseStatusCodePagesWithRedirects("/404");

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var ext = Path.GetExtension(ctx.File.PhysicalPath ?? "").ToLowerInvariant();
        if (ext is ".css" or ".js")
        {
            // Force revalidation on every request: browser sends ETag/If-Modified-Since,
            // server returns 304 if unchanged (fast) or the new file if it changed.
            ctx.Context.Response.Headers.CacheControl = "no-cache, must-revalidate";
        }
        else
        {
            // Images, fonts, etc. are immutable once deployed — cache 1 year.
            ctx.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
        }
    }
});

app.UseAntiforgery();
#endregion

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
