using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using FiyiPOS.Areas.FiyiPOS.Common.ConfigBack.Repositories;

namespace FiyiPOS.Mailing
{
    public class Email : IEmail
    {
        private readonly ConfigRepository    _config;
        private readonly IWebHostEnvironment _env;

        public Email(ConfigRepository config, IWebHostEnvironment env)
        {
            _config = config;
            _env    = env;
        }

        // ── Public methods ────────────────────────────────────────────────

        public async Task SendSignUpEmailAsync(string toEmail, string fullName, string activationUrl)
        {
            string body = BuildBody("new-user.html",
                ("{*1*}", $"Hola, {fullName}"),
                ("{*2*}", toEmail),
                ("{*3*}", activationUrl));

            await SendAsync(toEmail, fullName, "Activá tu cuenta en FiyiPOS", body);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetUrl)
        {
            string body = BuildBody("reset-password.html",
                ("{*1*}", $"Hola, {fullName}"),
                ("{*2*}", resetUrl));

            await SendAsync(toEmail, fullName, "Restablecé tu contraseña de FiyiPOS", body);
        }

        public async Task SendLowStockAlertAsync(
            string toEmail,
            string toName,
            string storeName,
            DateTime saleDate,
            List<(string Name, decimal Stock, decimal MinStock)> products)
        {
            string rows = string.Concat(products.Select((p, i) =>
                $"<tr style=\"background:{(i % 2 == 0 ? "#ffffff" : "#f8fafc")};\">" +
                $"<td style=\"padding:10px 16px; color:#1e293b; border-bottom:1px solid #f1f5f9;\">{p.Name}</td>" +
                $"<td style=\"padding:10px 16px; text-align:center; color:#dc2626; font-weight:700; border-bottom:1px solid #f1f5f9;\">{p.Stock:N2}</td>" +
                $"<td style=\"padding:10px 16px; text-align:center; color:#64748b; border-bottom:1px solid #f1f5f9;\">{p.MinStock:N2}</td>" +
                $"</tr>"));

            string body = BuildBody("low-stock-alert.html",
                ("{*1*}", toName),
                ("{*2*}", storeName),
                ("{*3*}", saleDate.ToString("dd/MM/yyyy HH:mm")),
                ("{*4*}", rows));

            await SendAsync(toEmail, toName, $"Alerta de stock bajo — {storeName}", body);
        }

        public async Task SendErrorAlertAsync(string message, string source, string stackTrace, DateTime occurredAt)
        {
            string? adminEmail = await Get("smtp_admin_email");
            if (string.IsNullOrWhiteSpace(adminEmail)) return;

            string Enc(string? s) => WebUtility.HtmlEncode(s ?? "(vacío)");

            string shortMsg    = string.IsNullOrEmpty(message) ? "(sin mensaje)"
                               : message.Length > 90 ? message[..90] + "…" : message;
            string truncStack  = string.IsNullOrEmpty(stackTrace) ? "(sin stack trace)"
                               : stackTrace.Length > 3500 ? stackTrace[..3500] + "\n… (truncado)" : stackTrace;

            string html = $@"<!DOCTYPE html>
<html lang='es'><head><meta charset='UTF-8'></head>
<body style='font-family:Segoe UI,Arial,sans-serif;background:#f8fafc;padding:24px;margin:0;'>
  <div style='max-width:700px;margin:0 auto;background:#fff;border-radius:10px;
              box-shadow:0 2px 14px rgba(0,0,0,.12);overflow:hidden;'>
    <div style='background:#dc2626;color:#fff;padding:20px 28px;'>
      <div style='font-size:1.15rem;font-weight:700;'>&#9888; Error en FiyiPOS</div>
      <div style='font-size:.85rem;opacity:.85;margin-top:4px;'>{occurredAt:dd/MM/yyyy HH:mm:ss}</div>
    </div>
    <div style='padding:28px;'>
      <table style='width:100%;border-collapse:collapse;font-size:.9rem;'>
        <tr>
          <td style='padding:9px 0;color:#64748b;width:110px;vertical-align:top;font-weight:600;'>Mensaje</td>
          <td style='padding:9px 0;font-weight:700;color:#1e293b;'>{Enc(message)}</td>
        </tr>
        <tr style='border-top:1px solid #f1f5f9;'>
          <td style='padding:9px 0;color:#64748b;vertical-align:top;font-weight:600;'>Origen</td>
          <td style='padding:9px 0;color:#475569;'>{Enc(source)}</td>
        </tr>
        <tr style='border-top:1px solid #f1f5f9;'>
          <td style='padding:9px 0;color:#64748b;vertical-align:top;font-weight:600;'>Stack trace</td>
          <td style='padding:9px 0;'>
            <pre style='font-family:monospace;font-size:.76rem;color:#374151;background:#f1f5f9;
                        padding:12px;border-radius:6px;white-space:pre-wrap;word-break:break-all;
                        margin:0;'>{Enc(truncStack)}</pre>
          </td>
        </tr>
      </table>
    </div>
    <div style='background:#f8fafc;padding:12px 28px;font-size:.78rem;color:#94a3b8;border-top:1px solid #e2e8f0;'>
      Alerta automática de FiyiPOS &mdash; {occurredAt:dd/MM/yyyy}
    </div>
  </div>
</body></html>";

            await SendAsync(adminEmail, "Administrador", $"[FiyiPOS Error] {shortMsg}", html);
        }

        public async Task SendNewUserAlertAsync(string userName, string userEmail, string role, DateTime registeredAt)
        {
            string? adminEmail = await Get("smtp_admin_email");
            if (string.IsNullOrWhiteSpace(adminEmail)) return;

            string Enc(string? s) => WebUtility.HtmlEncode(s ?? "");

            string roleLabel = role switch
            {
                "admin"      => "Administrador",
                "supervisor" => "Supervisor",
                "cashier"    => "Cajero",
                _            => Enc(role)
            };

            string html = $@"<!DOCTYPE html>
<html lang='es'><head><meta charset='UTF-8'></head>
<body style='font-family:Segoe UI,Arial,sans-serif;background:#f8fafc;padding:24px;margin:0;'>
  <div style='max-width:580px;margin:0 auto;background:#fff;border-radius:10px;
              box-shadow:0 2px 14px rgba(0,0,0,.12);overflow:hidden;'>
    <div style='background:#1aacb1;color:#fff;padding:20px 28px;'>
      <div style='font-size:1.15rem;font-weight:700;'>&#128100; Nuevo usuario registrado</div>
      <div style='font-size:.85rem;opacity:.85;margin-top:4px;'>{registeredAt:dd/MM/yyyy HH:mm:ss}</div>
    </div>
    <div style='padding:28px;'>
      <table style='width:100%;border-collapse:collapse;font-size:.9rem;'>
        <tr>
          <td style='padding:9px 0;color:#64748b;width:110px;font-weight:600;'>Nombre</td>
          <td style='padding:9px 0;color:#1e293b;font-weight:700;'>{Enc(userName)}</td>
        </tr>
        <tr style='border-top:1px solid #f1f5f9;'>
          <td style='padding:9px 0;color:#64748b;font-weight:600;'>Usuario / Email</td>
          <td style='padding:9px 0;color:#1e293b;'>{Enc(userEmail)}</td>
        </tr>
        <tr style='border-top:1px solid #f1f5f9;'>
          <td style='padding:9px 0;color:#64748b;font-weight:600;'>Rol asignado</td>
          <td style='padding:9px 0;color:#1e293b;'>{roleLabel}</td>
        </tr>
      </table>
    </div>
    <div style='background:#f8fafc;padding:12px 28px;font-size:.78rem;color:#94a3b8;border-top:1px solid #e2e8f0;'>
      Alerta automática de FiyiPOS &mdash; {registeredAt:dd/MM/yyyy}
    </div>
  </div>
</body></html>";

            await SendAsync(adminEmail, "Administrador", $"[FiyiPOS] Nuevo usuario — {userName}", html);
        }

        public async Task SendCashCutDiscrepancyAlertAsync(
            string toEmail, string toName,
            string storeName, string operatorName,
            decimal difference, decimal expectedAmount, decimal actualAmount,
            DateTime closedAt)
        {
            string Enc(string? s) => WebUtility.HtmlEncode(s ?? "");

            string diffSign  = difference > 0 ? "+" : "";
            string diffColor = difference > 0 ? "#f59e0b" : "#dc2626";
            string diffLabel = difference > 0 ? "Sobrante" : "Faltante";

            string html = $@"<!DOCTYPE html>
<html lang='es'><head><meta charset='UTF-8'></head>
<body style='font-family:Segoe UI,Arial,sans-serif;background:#f8fafc;padding:24px;margin:0;'>
  <div style='max-width:640px;margin:0 auto;background:#fff;border-radius:10px;
              box-shadow:0 2px 14px rgba(0,0,0,.12);overflow:hidden;'>
    <div style='background:{diffColor};color:#fff;padding:20px 28px;'>
      <div style='font-size:1.15rem;font-weight:700;'>&#9888; Diferencia en corte de caja</div>
      <div style='font-size:.85rem;opacity:.85;margin-top:4px;'>{Enc(storeName)} &mdash; {closedAt:dd/MM/yyyy HH:mm}</div>
    </div>
    <div style='padding:28px;'>
      <table style='width:100%;border-collapse:collapse;font-size:.9rem;'>
        <tr>
          <td style='padding:9px 0;color:#64748b;width:140px;font-weight:600;'>Operador</td>
          <td style='padding:9px 0;color:#1e293b;font-weight:700;'>{Enc(operatorName)}</td>
        </tr>
        <tr style='border-top:1px solid #f1f5f9;'>
          <td style='padding:9px 0;color:#64748b;font-weight:600;'>Monto esperado</td>
          <td style='padding:9px 0;color:#1e293b;'>${expectedAmount:N2}</td>
        </tr>
        <tr style='border-top:1px solid #f1f5f9;'>
          <td style='padding:9px 0;color:#64748b;font-weight:600;'>Monto contado</td>
          <td style='padding:9px 0;color:#1e293b;'>${actualAmount:N2}</td>
        </tr>
        <tr style='border-top:1px solid #f1f5f9;'>
          <td style='padding:9px 0;color:#64748b;font-weight:600;'>{diffLabel}</td>
          <td style='padding:9px 0;font-weight:700;font-size:1.1rem;color:{diffColor};'>{diffSign}${Math.Abs(difference):N2}</td>
        </tr>
      </table>
    </div>
    <div style='background:#f8fafc;padding:12px 28px;font-size:.78rem;color:#94a3b8;border-top:1px solid #e2e8f0;'>
      Alerta automática de FiyiPOS &mdash; {closedAt:dd/MM/yyyy}
    </div>
  </div>
</body></html>";

            await SendAsync(toEmail, toName, $"[FiyiPOS] Diferencia en corte — {storeName}", html);
        }

        // ── Core send logic ───────────────────────────────────────────────

        private async Task SendAsync(string toEmail, string toName,
                                     string subject, string htmlBody)
        {
            if (await GetBool("smtp_test_mode"))
            {
                await WriteDebugFileAsync(toEmail, subject, htmlBody);
                return;
            }

            string host     = await Get("smtp_host")       ?? "localhost";
            int    port     = int.TryParse(await Get("smtp_port"), out int p) ? p : 587;
            bool   useSsl   = await GetBool("smtp_ssl");
            string user     = await Get("smtp_user")       ?? "";
            string pass     = await Get("smtp_password")   ?? "";
            string fromAddr = await Get("smtp_from_email") ?? user;
            string fromName = await Get("smtp_from_name")  ?? "FiyiPOS";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddr));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body    = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port,
                useSsl ? SecureSocketOptions.SslOnConnect
                       : SecureSocketOptions.StartTlsWhenAvailable);

            if (!string.IsNullOrEmpty(user))
                await client.AuthenticateAsync(user, pass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        // ── Template builder ──────────────────────────────────────────────

        private string BuildBody(string templateFile,
                                 params (string placeholder, string value)[] replacements)
        {
            string path = Path.Combine(_env.WebRootPath, "mailing-templates", templateFile);
            string html = File.Exists(path)
                ? File.ReadAllText(path)
                : "<p>{*2*}</p>";

            foreach (var (placeholder, value) in replacements)
                html = html.Replace(placeholder, value);

            return html;
        }

        // ── Test-mode: dump to file ───────────────────────────────────────

        private async Task WriteDebugFileAsync(string toEmail, string subject, string html)
        {
            string dir = Path.Combine(_env.WebRootPath, "email-debug");
            Directory.CreateDirectory(dir);

            string safe     = toEmail.Replace("@", "_at_").Replace(".", "_");
            string filename = $"{DateTime.Now:yyyyMMdd-HHmmss}_{safe}.html";
            string content  = $"<!-- TO: {toEmail} | SUBJECT: {subject} -->\n{html}";

            await File.WriteAllTextAsync(Path.Combine(dir, filename), content);
        }

        // ── Config helpers ────────────────────────────────────────────────

        private Task<string?> Get(string key)      => _config.GetValueAsync(key);
        private async Task<bool> GetBool(string key) => await Get(key) == "1";
    }
}
