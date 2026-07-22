using System.Net;
using System.Text;
using FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.DTOs;
using static FiyiPOS.Areas.FiyiPOS.Common.CashCutBack.DTOs.CashCutReportDTO;

namespace FiyiPOS.Services.FiyiPOS
{
    public static class CashCutReportBuilder
    {
        public static string BuildHtml(CashCutReportDTO r)
        {
            static string M(decimal v) => $"${v:N2}";
            static string Esc(string? s) => WebUtility.HtmlEncode(s ?? "");

            var sb = new StringBuilder();

            string diffClass = r.Difference < 0 ? "neg" : r.Difference > 0 ? "pos" : "zero";
            string diffSign  = r.Difference > 0 ? "+" : "";

            sb.Append(@"<!DOCTYPE html>
<html lang='es'>
<head>
<meta charset='UTF-8'>
<title>Informe de Corte #").Append(r.CashCutId).Append(" — ").Append(Esc(r.StoreName)).Append(@"</title>
<style>
*{box-sizing:border-box;margin:0;padding:0}
body{font-family:'Segoe UI',Arial,sans-serif;font-size:12px;color:#1a1a1a;background:#fff;padding:24px 28px}
h2{font-size:13px;font-weight:700;color:#1565c0;border-bottom:2px solid #1565c0;padding-bottom:4px;margin:22px 0 10px;text-transform:uppercase;letter-spacing:.5px}
.hdr{display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:18px;padding-bottom:14px;border-bottom:3px solid #1565c0}
.hdr-title{font-size:20px;font-weight:800;color:#1565c0;margin-bottom:4px}
.hdr-meta{font-size:11px;color:#555;margin-top:3px}
.hdr-right{text-align:right;font-size:11px;color:#777}
.kpi-row{display:grid;grid-template-columns:repeat(3,1fr);gap:10px;margin-bottom:18px}
.kpi{border:1px solid #e0e0e0;border-radius:6px;padding:10px 14px}
.kpi-label{font-size:10px;color:#888;text-transform:uppercase;letter-spacing:.4px}
.kpi-value{font-size:18px;font-weight:700;margin-top:3px}
.pos{color:#059669}.neg{color:#dc2626}.zero{color:#9ca3af}
table{width:100%;border-collapse:collapse;font-size:11px;margin-bottom:4px}
th{background:#f3f4f6;padding:6px 10px;text-align:left;font-weight:600;border-bottom:2px solid #e0e0e0;font-size:10px;text-transform:uppercase;letter-spacing:.3px}
td{padding:5px 10px;border-bottom:1px solid #f0f0f0;vertical-align:middle}
.tr-last td{border-bottom:2px solid #e0e0e0;font-weight:700}
.r{text-align:right}.c{text-align:center}
.badge{padding:2px 8px;border-radius:99px;font-size:10px;font-weight:600}
.bin{background:#d1fae5;color:#065f46}.bout{background:#fee2e2;color:#991b1b}
.bcredit{background:#ede9fe;color:#5b21b6}.bcash{background:#dbeafe;color:#1d4ed8}
.notes{background:#fffbeb;border:1px solid #fde68a;border-radius:6px;padding:8px 12px;font-size:11px;margin-bottom:16px}
.footer{margin-top:24px;padding-top:10px;border-top:1px solid #e0e0e0;font-size:10px;color:#aaa;text-align:center}
.no-print{text-align:center;margin-bottom:22px}
.btn-p{background:#1565c0;color:#fff;border:none;padding:9px 26px;border-radius:6px;font-size:13px;cursor:pointer;font-weight:600}
@media print{.no-print{display:none}body{padding:0}}
</style></head><body>
<div class='no-print'><button class='btn-p' onclick='window.print()'>&#128438; Imprimir / Guardar PDF</button></div>
");

            sb.Append("<div class='hdr'><div><div class='hdr-title'>INFORME DE CORTE DE CAJA</div>");
            sb.Append($"<div class='hdr-meta'>Sucursal: <strong>{Esc(r.StoreName)}</strong> &nbsp;·&nbsp; Operador: <strong>{Esc(r.OperatorName)}</strong></div>");
            sb.Append($"<div class='hdr-meta'>Período: <strong>{r.OpenedAt:dd/MM/yyyy HH:mm}</strong> → <strong>{r.ClosedAt:dd/MM/yyyy HH:mm}</strong></div>");
            sb.Append("</div>");
            sb.Append($"<div class='hdr-right'>Generado el {r.GeneratedAt:dd/MM/yyyy HH:mm}<br>Corte N° {r.CashCutId}</div></div>");

            sb.Append("<h2>Resumen del cierre</h2><div class='kpi-row'>");
            void Kpi(string label, string val, string cls = "") =>
                sb.Append($"<div class='kpi'><div class='kpi-label'>{label}</div><div class='kpi-value {cls}'>{val}</div></div>");
            Kpi("Fondo inicial",    M(r.OpeningAmount));
            Kpi("Total recaudado", M(r.PaymentLines.Sum(l => l.Total)));
            Kpi("Ventas",          r.TotalSales.ToString());
            Kpi("Monto esperado",  M(r.ExpectedAmount));
            Kpi("Monto contado",   M(r.ActualAmount));
            Kpi("Diferencia",      $"{diffSign}{M(r.Difference)}", diffClass);
            sb.Append("</div>");

            decimal revenueGross  = r.TotalSalesGross;
            decimal revenueNet    = r.TotalSalesNet;
            decimal profitNet     = revenueNet   - r.TotalCOGS;
            decimal marginNetPct  = revenueNet   > 0 ? Math.Round(profitNet   / revenueNet   * 100, 1) : 0;
            decimal profitGross   = revenueGross - r.TotalCOGS;
            decimal marginGrossPct= revenueGross > 0 ? Math.Round(profitGross / revenueGross * 100, 1) : 0;
            string  profClassNet   = profitNet   >= 0 ? "pos" : "neg";
            string  profClassGross = profitGross >= 0 ? "pos" : "neg";

            sb.Append("<h2>Rentabilidad del período (COGS)</h2><div class='kpi-row'>");
            Kpi("Ventas con IVA",             M(revenueGross));
            Kpi("Ventas sin IVA",             M(revenueNet));
            Kpi("Costo de productos (COGS)", M(r.TotalCOGS));
            Kpi("Ganancia bruta (sin IVA)",   M(profitNet),   profClassNet);
            Kpi("Margen bruto (sin IVA)",     $"{marginNetPct}%", profClassNet);
            Kpi("Ganancia bruta (con IVA)",   M(profitGross), profClassGross);
            Kpi("Margen bruto (con IVA)",     $"{marginGrossPct}%", profClassGross);
            sb.Append("</div>");
            sb.Append("<p style='font-size:10px;color:#94a3b8;margin:-10px 0 16px;'>El margen sin IVA es el indicador contable recomendado: el IVA cobrado no es ingreso propio.</p>");

            if (!string.IsNullOrWhiteSpace(r.Notes))
                sb.Append($"<div class='notes'><strong>Notas:</strong> {Esc(r.Notes)}</div>");

            sb.Append("<h2>Recaudación por forma de pago</h2><table><thead><tr><th>Método</th><th class='c'>Efectivo</th><th class='c'>Transacciones</th><th class='r'>Total</th></tr></thead><tbody>");
            foreach (var l in r.PaymentLines)
                sb.Append($"<tr><td>{Esc(l.MethodName)}</td><td class='c'>{(l.IsCash ? "<span class='badge bcash'>Sí</span>" : "—")}</td><td class='c'>{l.Count}</td><td class='r'>{M(l.Total)}</td></tr>");
            decimal pmTotal = r.PaymentLines.Sum(l => l.Total);
            sb.Append($"<tr class='tr-last'><td colspan='2'></td><td class='c'>{r.PaymentLines.Sum(l => l.Count)}</td><td class='r'>{M(pmTotal)}</td></tr>");
            sb.Append("</tbody></table>");

            if (r.Movements.Any())
            {
                decimal movIn  = r.Movements.Where(m => m.Type == "IN").Sum(m => m.Amount);
                decimal movOut = r.Movements.Where(m => m.Type == "OUT").Sum(m => m.Amount);
                sb.Append($"<h2>Movimientos de efectivo ({r.Movements.Count})</h2><table><thead><tr><th>Tipo</th><th>Fecha y hora</th><th>Concepto</th><th>Operador</th><th class='r'>Importe</th></tr></thead><tbody>");
                foreach (var m in r.Movements)
                {
                    string badge = m.Type == "IN" ? "<span class='badge bin'>Entrada</span>" : "<span class='badge bout'>Salida</span>";
                    string sign  = m.Type == "IN" ? "+" : "−";
                    sb.Append($"<tr><td>{badge}</td><td>{m.Date:dd/MM/yyyy HH:mm}</td><td>{Esc(m.Reason)}</td><td>{Esc(m.Operator)}</td><td class='r'>{sign}{M(m.Amount)}</td></tr>");
                }
                sb.Append($"<tr class='tr-last'><td colspan='3'></td><td>Entradas: {M(movIn)} / Salidas: {M(movOut)}</td><td class='r'>{M(movIn - movOut)}</td></tr>");
                sb.Append("</tbody></table>");
            }

            sb.Append($"<h2>Listado de ventas ({r.Sales.Count})</h2><table><thead><tr><th>Folio</th><th>Hora</th><th>Operador</th><th>Cliente</th><th>Método de pago</th><th class='r'>Total</th></tr></thead><tbody>");
            foreach (var s in r.Sales)
            {
                string clientCell = string.IsNullOrWhiteSpace(s.Client) ? "—" : Esc(s.Client);
                string pmCell     = s.IsCredit ? "<span class='badge bcredit'>Crédito</span>" : Esc(s.PaymentMethod);
                sb.Append($"<tr><td>{Esc(s.Ticket)}</td><td>{s.Date:HH:mm}</td><td>{Esc(s.Operator)}</td><td>{clientCell}</td><td>{pmCell}</td><td class='r'>{M(s.Total)}</td></tr>");
            }
            sb.Append($"<tr class='tr-last'><td colspan='5'></td><td class='r'>{M(r.Sales.Sum(s => s.Total))}</td></tr>");
            sb.Append("</tbody></table>");

            if (r.ProductsByCategory.Any())
            {
                int     totalLines = r.ProductsByCategory.Sum(g => g.Lines.Count);
                decimal grandTotal = r.ProductsByCategory.Sum(g => g.Lines.Sum(l => l.TotalSubtotal));
                sb.Append($"<h2>Productos vendidos ({totalLines} producto{(totalLines != 1 ? "s" : "")} en {r.ProductsByCategory.Count} categoría{(r.ProductsByCategory.Count != 1 ? "s" : "")})</h2>");

                foreach (var group in r.ProductsByCategory)
                {
                    decimal catTotal = group.Lines.Sum(l => l.TotalSubtotal);
                    sb.Append("<div style='font-size:11px;font-weight:700;color:#1565c0;background:#eff6ff;padding:5px 10px;border-left:3px solid #1565c0;margin-bottom:2px;margin-top:10px;display:flex;justify-content:space-between;align-items:center;gap:8px;'>");
                    sb.Append($"<span>{Esc(group.CategoryName)}</span>");
                    if (group.PaymentBreakdown.Any())
                    {
                        sb.Append("<span style='display:flex;gap:4px;flex-wrap:wrap;'>");
                        foreach (var pm in group.PaymentBreakdown)
                            sb.Append($"<span style='font-size:10px;font-weight:500;background:#fff;border:1px solid #93c5fd;border-radius:3px;padding:1px 6px;color:#1e40af;'>{Esc(pm.MethodName)}: {M(pm.Subtotal)}</span>");
                        sb.Append("</span>");
                    }
                    sb.Append("</div>");
                    sb.Append("<table><thead><tr><th>Producto</th><th class='r'>Cantidad</th><th class='r'>Importe total</th></tr></thead><tbody>");
                    foreach (var line in group.Lines)
                        sb.Append($"<tr><td>{Esc(line.ProductName)}</td><td class='r'>{line.TotalQty:N2}</td><td class='r'>{M(line.TotalSubtotal)}</td></tr>");
                    sb.Append($"<tr class='tr-last'><td><em>Subtotal {Esc(group.CategoryName)}</em></td><td></td><td class='r'>{M(catTotal)}</td></tr>");
                    sb.Append("</tbody></table>");
                }

                sb.Append($"<div style='text-align:right;font-size:12px;font-weight:700;padding:6px 10px;border-top:2px solid #1565c0;margin-top:4px'>Total general de productos: {M(grandTotal)}</div>");
            }

            if (r.Returns.Any())
            {
                sb.Append($"<h2>Devoluciones ({r.Returns.Count})</h2><table><thead><tr><th>Ticket original</th><th>Fecha y hora</th><th>Motivo</th><th>Operador</th><th class='r'>Importe devuelto</th></tr></thead><tbody>");
                foreach (var ret in r.Returns)
                    sb.Append($"<tr><td>{Esc(ret.OriginalTicket)}</td><td>{ret.Date:dd/MM/yyyy HH:mm}</td><td>{Esc(ret.Reason)}</td><td>{Esc(ret.Operator)}</td><td class='r'>{M(ret.Amount)}</td></tr>");
                sb.Append($"<tr class='tr-last'><td colspan='4'></td><td class='r'>{M(r.Returns.Sum(x => x.Amount))}</td></tr>");
                sb.Append("</tbody></table>");
            }

            if (r.Credits.Any())
            {
                sb.Append($"<h2>Créditos generados ({r.Credits.Count})</h2><table><thead><tr><th>Cliente</th><th>Fecha</th><th class='r'>Importe original</th><th class='r'>Saldo pendiente</th></tr></thead><tbody>");
                foreach (var cr in r.Credits)
                    sb.Append($"<tr><td>{Esc(cr.Client)}</td><td>{cr.Date:dd/MM/yyyy HH:mm}</td><td class='r'>{M(cr.Original)}</td><td class='r'>{M(cr.Balance)}</td></tr>");
                sb.Append("</tbody></table>");
            }

            if (r.Purchases.Any())
            {
                decimal purchTotal = r.Purchases.Sum(p => p.Total);
                int     confirmed  = r.Purchases.Count(p => p.IsConfirmed);
                int     pending    = r.Purchases.Count - confirmed;
                sb.Append($"<h2>Compras a proveedores ({r.Purchases.Count})</h2>");
                sb.Append("<table><thead><tr><th>Fecha</th><th>Proveedor</th><th>N° factura</th><th class='c'>Productos</th><th class='r'>Subtotal</th><th class='r'>IVA</th><th class='r'>Total</th><th class='c'>Estado</th></tr></thead><tbody>");
                foreach (var p in r.Purchases)
                {
                    string statusBadge = p.IsConfirmed
                        ? "<span class='badge bin'>Confirmada</span>"
                        : "<span class='badge bout'>Pendiente</span>";
                    sb.Append($"<tr><td>{p.Date:dd/MM/yyyy}</td><td>{Esc(p.SupplierName)}</td><td>{Esc(p.Invoice ?? "—")}</td><td class='c'>{p.ItemCount}</td><td class='r'>{M(p.Subtotal)}</td><td class='r'>{M(p.TaxAmount)}</td><td class='r'>{M(p.Total)}</td><td class='c'>{statusBadge}</td></tr>");
                }
                sb.Append($"<tr class='tr-last'><td colspan='4'>{confirmed} confirmada{(confirmed != 1 ? "s" : "")} · {pending} pendiente{(pending != 1 ? "s" : "")}</td><td colspan='2'></td><td class='r'>{M(purchTotal)}</td><td></td></tr>");
                sb.Append("</tbody></table>");
            }

            sb.Append($"<div class='footer'>FiyiPOS &nbsp;·&nbsp; Informe generado el {r.GeneratedAt:dd/MM/yyyy} a las {r.GeneratedAt:HH:mm}</div>");
            sb.Append("<div class='no-print' style='margin-top:20px'><button class='btn-p' onclick='window.print()'>&#128438; Imprimir / Guardar PDF</button></div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }
    }
}
