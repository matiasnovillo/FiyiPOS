/*
 *
 * Coded by fiyistack.com
 * Copyright © 2026
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 */

using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;

namespace FiyiPOS.Services.FiyiPOS
{
    public class PosSessionService
    {
        public long        SystemUserId     { get; private set; }
        public long        StoreId          { get; private set; }
        public string      StoreName        { get; private set; } = "";
        public string      OperatorName     { get; private set; } = "";
        public string      Role             { get; private set; } = "";
        public List<Store> AccessibleStores { get; private set; } = new();
        public DateTime?   BillingDueDate   { get; private set; }

        public bool IsAuthenticated  => SystemUserId > 0;
        public bool CanSwitchStore   => AccessibleStores.Count > 1;
        public bool ForgotCheckout   { get; private set; }

        public void SetForgotCheckout(bool value) => ForgotCheckout = value;

        public int? BillingDaysLeft =>
            BillingDueDate.HasValue
                ? (int)(BillingDueDate.Value.Date - DateTime.Today).TotalDays
                : null;

        public bool ShowBillingWarning =>
            BillingDaysLeft.HasValue && BillingDaysLeft.Value <= 7;

        // ── Permissions ────────────────────────────────
        private static readonly Dictionary<string, HashSet<string>> _permissions = new()
        {
            ["cashier"] = new()
            {
                "ventas", "creditos", "movimientos", "clientes",
                "inventario", "facturas", "devoluciones", "consumointerno",
                "corte", "dashboard", "comida"
            },
            ["supervisor"] = new()
            {
                "ventas", "creditos", "movimientos", "clientes", "productos",
                "inventario", "facturas", "proveedores",
                "compras", "corte", "reportes", "operadores", "devoluciones", "consumointerno", "promociones", "notepad",
                "dashboard", "comida"
            },
            ["admin"] = new()
            {
                "ventas", "creditos", "movimientos", "clientes", "productos",
                "inventario", "facturas", "proveedores",
                "compras", "corte", "reportes", "configuracion", "operadores", "sucursales", "devoluciones", "consumointerno", "promociones", "esquema", "notepad",
                "aichat", "dashboard", "comida", "asistencias", "categorias", "unidades"
            },
        };

        public bool CanAccess(string section) =>
            _permissions.TryGetValue(Role, out var allowed) && allowed.Contains(section);

        public string FirstAccessibleSection() =>
            _permissions.TryGetValue(Role, out var allowed) && allowed.Contains("ventas")
                ? "ventas"
                : _permissions.TryGetValue(Role, out var a2) && a2.Count > 0
                    ? a2.First()
                    : "ventas";

        public void SetSession(long systemUserId, long storeId,
                               string storeName, string operatorName, string role)
        {
            SystemUserId = systemUserId;
            StoreId      = storeId;
            StoreName    = storeName;
            OperatorName = operatorName;
            Role         = role;
            OnSessionChanged?.Invoke();
        }

        public void SetAccessibleStores(List<Store> stores)
            => AccessibleStores = stores;

        public void SetBillingDueDate(DateTime? date)
            => BillingDueDate = date;

        public event Action? OnSessionChanged;
        public event Action? OnStoreChanged;

        public void SwitchStore(long storeId, string storeName)
        {
            StoreId   = storeId;
            StoreName = storeName;
            OnStoreChanged?.Invoke();
        }

        public void ClearSession()
        {
            SystemUserId     = 0;
            StoreId          = 0;
            StoreName        = "";
            OperatorName     = "";
            Role             = "";
            AccessibleStores = new();
            BillingDueDate   = null;
            ForgotCheckout   = false;
        }
    }
}
