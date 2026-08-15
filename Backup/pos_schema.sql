-- ====================================================================
-- POS - Punto de Venta
-- Motor: MS SQL Server
-- Convenciones:
--   · Tablas en singular
--   · PK/FK: EntidadId  (BIGINT)
--   · Fechas: DATETIME
--   · Booleanos: TINYINT  (0 = false, 1 = true)
--   · Decimales: NUMERIC(18,2)
--   · Tiempos: TIME(3)
--   · Texto corto: VARCHAR(N)
--   · Texto libre: VARCHAR(MAX)
-- ====================================================================

-- ────────────────────────────────────────────────────────────────────
-- 1. Store — Tienda / Sucursal
--    Representa el local físico donde opera el POS.
--    Permite escalar a múltiples sucursales sin rediseño.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Store (
    StoreId        BIGINT        IDENTITY(1,1) PRIMARY KEY,
    Name           VARCHAR(150)  NOT NULL,   -- Nombre visible del local ("Librería Central")
    Address        VARCHAR(300)  NULL,       -- Dirección física completa
    Phone          VARCHAR(30)   NULL,       -- Teléfono de atención al cliente
    TaxId          VARCHAR(30)   NULL,       -- RFC / CUIT / NIF para facturación fiscal
    LogoUrl        VARCHAR(500)  NULL,       -- URL del logo que aparece en tickets impresos
    Active         TINYINT       NOT NULL DEFAULT 1,
    CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE(),
    BillingDueDate DATETIME      NULL        -- Fecha límite de pago de suscripción mensual
);

-- ────────────────────────────────────────────────────────────────────
-- 2. Category — Categoría de producto
--    Agrupa los productos en familias (Útiles, Golosinas, Revistas…).
--    Facilita búsquedas, reportes y acomodo visual del catálogo.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Category (
    CategoryId  BIGINT        IDENTITY(1,1) PRIMARY KEY,
    Name        VARCHAR(100)  NOT NULL,   -- Nombre de la categoría ("Papelería", "Snacks")
    Description VARCHAR(300)  NULL,       -- Descripción opcional para el operador
    Active      TINYINT       NOT NULL DEFAULT 1
);

-- ────────────────────────────────────────────────────────────────────
-- 3. Unit — Unidad de medida
--    Define cómo se vende y mide cada producto (pieza, kg, litro…).
--    Necesario para el cálculo correcto de cantidades en inventario.
--    Multi-tenant (2026-07-22): cada Store administra sus propias
--    unidades desde Administrador → Unidades de medida (solo rol admin).
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Unit (
    UnitId          BIGINT       IDENTITY(1,1) PRIMARY KEY,
    StoreId         BIGINT       NOT NULL REFERENCES Store(StoreId),
    Name            VARCHAR(50)  NOT NULL,   -- Nombre completo ("Kilogramo", "Pieza")
    Abbreviation    VARCHAR(10)  NOT NULL,   -- Abreviación que aparece en tickets ("kg", "pz")
    Active          TINYINT      NOT NULL DEFAULT 1
);

-- ────────────────────────────────────────────────────────────────────
-- 4. Supplier — Proveedor
--    Empresas o personas que surten el inventario.
--    Se vincula a compras y entradas de mercancía.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Supplier (
    SupplierId               BIGINT        IDENTITY(1,1) PRIMARY KEY,
    StoreId                  BIGINT        NOT NULL REFERENCES Store(StoreId),
    DateTimeCreation         DATETIME      NOT NULL,
    DateTimeLastModification DATETIME      NOT NULL,
    UserCreationId           BIGINT        NOT NULL,
    UserLastModificationId   BIGINT        NOT NULL,
    Active                   TINYINT       NOT NULL DEFAULT 1,
    Name                     VARCHAR(150)  NOT NULL,   -- Razón social o nombre del proveedor
    ContactName              VARCHAR(100)  NULL,        -- Nombre de la persona de contacto
    Phone                    VARCHAR(30)   NULL,        -- Teléfono principal
    Email                    VARCHAR(150)  NULL,        -- Correo para pedidos o facturas
    Address                  VARCHAR(300)  NULL,        -- Dirección del proveedor
    TaxId                    VARCHAR(30)   NULL,        -- RFC / CUIT para facturas
    Notes                    VARCHAR(MAX)  NULL         -- Notas internas (condiciones de pago, etc.)
);

-- ────────────────────────────────────────────────────────────────────
-- 5. SystemUser — Operador / Cajero / Administrador
--    Personas que usan el POS. "Le atiende: X" del encabezado.
--    El rol define qué puede ver o hacer cada usuario.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE SystemUser (
    SystemUserId                BIGINT        IDENTITY(1,1) PRIMARY KEY,
    Name                        VARCHAR(100)  NOT NULL,   -- Nombre completo del operador
    Username                    VARCHAR(200)  NOT NULL UNIQUE,
    PasswordHash                VARCHAR(255)  NOT NULL,   -- Hash de la contraseña (nunca texto plano)
    Role                        VARCHAR(20)   NOT NULL DEFAULT 'cashier',
                                                          -- 'admin' | 'cashier' | 'supervisor'
    Active                      TINYINT       NOT NULL DEFAULT 1,
    CreatedAt                   DATETIME      NOT NULL DEFAULT GETDATE(),
    ActivationToken             VARCHAR(50)   NULL,
    PasswordResetToken          VARCHAR(50)   NULL,       -- Token de un solo uso para /restablecer-password
    PasswordResetTokenExpiresAt DATETIME      NULL        -- Expira 1 hora después de solicitarse
);

-- ────────────────────────────────────────────────────────────────────
-- 5b. SystemUserStore — Asignación usuario → sucursal (N:M)
--     Permite que un operador acceda a múltiples sucursales.
--     Todos los roles usan esta tabla para determinar sus sucursales.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE SystemUserStore (
    SystemUserId    BIGINT  NOT NULL REFERENCES SystemUser(SystemUserId) ON DELETE CASCADE,
    StoreId         BIGINT  NOT NULL REFERENCES Store(StoreId)          ON DELETE CASCADE,
    PRIMARY KEY (SystemUserId, StoreId)
);

-- ────────────────────────────────────────────────────────────────────
-- 6. Client — Cliente
--    Personas que compran en el local.
--    Requerido para asignar créditos, emitir facturas y fidelización.
--    Multi-tenant: cada cliente pertenece a una tienda (StoreId).
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Client (
    ClientId                BIGINT          IDENTITY(1,1) PRIMARY KEY,
    StoreId                 BIGINT          NOT NULL REFERENCES Store(StoreId),
    DateTimeCreation        DATETIME        NOT NULL DEFAULT GETDATE(),
    DateTimeLastModification DATETIME       NOT NULL DEFAULT GETDATE(),
    UserCreationId          BIGINT          NOT NULL,
    UserLastModificationId  BIGINT          NOT NULL,
    Active                  TINYINT         NOT NULL DEFAULT 1,
    Nombres                 VARCHAR(200)    NOT NULL,
    Apellidos               VARCHAR(200)    NOT NULL,
    Telefono                VARCHAR(50)     NULL,
    Correo                  VARCHAR(400)    NULL,
    Domicilio               VARCHAR(500)    NULL,
    CUIT                    VARCHAR(30)     NULL,       -- CUIT / RFC / NIF fiscal
    Notas                   VARCHAR(MAX)    NULL,
    TieneCreditoAutorizado  TINYINT         NOT NULL DEFAULT 0,
    LimiteCredito           NUMERIC(18,2)   NOT NULL DEFAULT 0,
    SaldoCredito            NUMERIC(18,2)   NOT NULL DEFAULT 0
);

-- ────────────────────────────────────────────────────────────────────
-- 7. PaymentMethod — Método de pago
--    Catálogo de formas de cobro: efectivo, tarjeta, transferencia…
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE PaymentMethod (
    PaymentMethodId BIGINT       IDENTITY(1,1) PRIMARY KEY,
    Name            VARCHAR(50)  NOT NULL,   -- "Efectivo", "Tarjeta débito", "Transferencia"
    RequiresRef     TINYINT      NOT NULL DEFAULT 0,
                                             -- 1 = solicita número de referencia al cobrar
    IsCash          TINYINT      NOT NULL DEFAULT 0,
                                             -- 1 = método en efectivo físico (para cálculo de corte)
    Active          TINYINT      NOT NULL DEFAULT 1
);

-- ────────────────────────────────────────────────────────────────────
-- 8. SaleStatus — Estado de la venta
--    Catálogo pequeño: Completada, Pendiente, Cancelada.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE SaleStatus (
    SaleStatusId    INT           IDENTITY(1,1) PRIMARY KEY,
    Name            VARCHAR(50)   NOT NULL    -- "Completada" | "Pendiente" | "Cancelada"
);

-- ────────────────────────────────────────────────────────────────────
-- 9. PriceList — Lista de precios
--    Permite tener precios diferenciados: público general vs mayoreo
--    (botón F11 Mayoreo de la pantalla).
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE PriceList (
    PriceListId     BIGINT        IDENTITY(1,1) PRIMARY KEY,
    Name            VARCHAR(100)  NOT NULL,   -- "Precio Público", "Mayoreo", "VIP"
    Description     VARCHAR(300)  NULL,
    Active          TINYINT       NOT NULL DEFAULT 1
);

-- ────────────────────────────────────────────────────────────────────
-- 10. Product — Producto
--     El catálogo central del POS. Cada fila es un artículo vendible.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Product (
    ProductId           BIGINT          IDENTITY(1,1) PRIMARY KEY,
    StoreId             BIGINT          NOT NULL REFERENCES Store(StoreId),
    CategoryId          BIGINT          NULL REFERENCES Category(CategoryId),
    UnitId              BIGINT          NULL REFERENCES Unit(UnitId),
    Barcode             VARCHAR(50)     NULL,           -- Código de barras principal
    Name                VARCHAR(150)    NOT NULL,
    Description         VARCHAR(MAX)    NULL,            -- Descripción larga para catálogo
    SalePrice           NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Precio al público
    CostPrice           NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Costo de adquisición
    WholesalePrice      NUMERIC(18,2)   NULL,                -- Precio mayoreo (F11)
    Stock               NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Existencia actual
    MinStock            NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Mínimo para alerta de reorden
    MaxStock            NUMERIC(18,2)   NULL,                -- Máximo sugerido en almacén
    TaxRate             NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Tasa de impuesto en %
    AllowFractional     TINYINT         NOT NULL DEFAULT 0,  -- 1 = se puede vender en fracciones (ej: 0.5 kg)
    Active              TINYINT         NOT NULL DEFAULT 1,
    CreatedAt           DATETIME        NOT NULL DEFAULT GETDATE(),
    UpdatedAt           DATETIME        NULL,

    -- Guard: si el producto no permite fracciones, stocks deben ser enteros.
    CONSTRAINT CK_Product_StockInteger CHECK (
        AllowFractional = 1
        OR (
            Stock    = FLOOR(Stock)    AND
            MinStock = FLOOR(MinStock) AND
            (MaxStock IS NULL OR MaxStock = FLOOR(MaxStock))
        )
    )
);

-- ── Para bases de datos ya existentes, ejecutar en SSMS: ────────────
-- Paso 1 (opcional): limpiar datos decimales en productos no fraccionables
-- UPDATE Product
--     SET Stock    = FLOOR(Stock),
--         MinStock = FLOOR(MinStock),
--         MaxStock = CASE WHEN MaxStock IS NOT NULL THEN FLOOR(MaxStock) ELSE NULL END
-- WHERE AllowFractional = 0
--   AND (Stock <> FLOOR(Stock) OR MinStock <> FLOOR(MinStock)
--        OR (MaxStock IS NOT NULL AND MaxStock <> FLOOR(MaxStock)));
--
-- Paso 2: agregar el constraint
-- ALTER TABLE Product
--     ADD CONSTRAINT CK_Product_StockInteger CHECK (
--         AllowFractional = 1
--         OR (
--             Stock    = FLOOR(Stock)    AND
--             MinStock = FLOOR(MinStock) AND
--             (MaxStock IS NULL OR MaxStock = FLOOR(MaxStock))
--         )
--     );
-- ─────────────────────────────────────────────────────────────────────

-- ────────────────────────────────────────────────────────────────────
-- 11. ProductBarcode — Códigos de barras adicionales del producto
--     Un mismo producto puede tener varios códigos (presentaciones,
--     importado vs nacional, paquete individual vs caja).
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE ProductBarcode (
    ProductBarcodeId    BIGINT       IDENTITY(1,1) PRIMARY KEY,
    ProductId           BIGINT       NOT NULL REFERENCES Product(ProductId),
    Barcode             VARCHAR(50)  NOT NULL,   -- Código alternativo escaneable
    Active              TINYINT      NOT NULL DEFAULT 1
);

-- ────────────────────────────────────────────────────────────────────
-- 12. ProductPriceList — Precio del producto por lista
--     Tabla de intersección: define el precio de cada producto
--     en cada lista (mayoreo, VIP, etc.).
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE ProductPriceList (
    ProductPriceListId  BIGINT          IDENTITY(1,1) PRIMARY KEY,
    ProductId           BIGINT          NOT NULL REFERENCES Product(ProductId),
    PriceListId         BIGINT          NOT NULL REFERENCES PriceList(PriceListId),
    Price               NUMERIC(18,2)   NOT NULL,
    CONSTRAINT UQ_ProductPriceList UNIQUE (ProductId, PriceListId)
);

-- ────────────────────────────────────────────────────────────────────
-- 13. Sale — Venta (Ticket)
--     Cabecera de cada transacción. Un ticket = una fila aquí.
--     Los artículos van en SaleDetail.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Sale (
    SaleId              BIGINT          IDENTITY(1,1) PRIMARY KEY,
    StoreId             BIGINT          NOT NULL REFERENCES Store(StoreId),
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    ClientId            BIGINT          NULL     REFERENCES Client(ClientId),
    SaleStatusId        INT             NOT NULL REFERENCES SaleStatus(SaleStatusId),
    PriceListId         BIGINT          NULL,    -- Lista usada (NULL = precio público; sin FK hasta implementar PriceList)
    TicketNumber        VARCHAR(30)     NOT NULL UNIQUE, -- Número de folio del ticket
    SaleDate            DATETIME        NOT NULL DEFAULT GETDATE(),
    Subtotal            NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Suma de artículos antes de impuesto
    Discount            NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Descuento total aplicado
    TaxAmount           NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- IVA / impuesto calculado
    Total               NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Total a cobrar
    AmountPaid          NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Lo que entregó el cliente
    Change              NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Cambio devuelto
    Notes               VARCHAR(500)    NULL,
    IsPrinted           TINYINT         NOT NULL DEFAULT 0,  -- 1 = ticket ya fue impreso
    CreatedAt           DATETIME        NOT NULL DEFAULT GETDATE()
);

-- ────────────────────────────────────────────────────────────────────
-- 14. SaleDetail — Renglones del ticket
--     Cada línea de la tabla en pantalla (Cód. Barras, Descripción,
--     Precio, Cant., Importe, Existencia).
--     Guarda snapshot del precio y nombre al momento de la venta.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE SaleDetail (
    SaleDetailId        BIGINT          IDENTITY(1,1) PRIMARY KEY,
    SaleId              BIGINT          NOT NULL REFERENCES Sale(SaleId),
    ProductId           BIGINT          NOT NULL REFERENCES Product(ProductId),
    BarcodeSnapshot     VARCHAR(50)     NULL,           -- Código escaneado en ese momento
    ProductNameSnapshot VARCHAR(150)    NOT NULL,       -- Nombre al momento de la venta (histórico)
    Quantity            NUMERIC(18,2)   NOT NULL,       -- Cant. vendida (permite fracciones)
    UnitPrice           NUMERIC(18,2)   NOT NULL,       -- Precio unitario aplicado
    Discount            NUMERIC(18,2)   NOT NULL DEFAULT 0,
    TaxRate             NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- % de impuesto del producto
    Subtotal            NUMERIC(18,2)   NOT NULL        -- Quantity * UnitPrice - Discount
);

-- ────────────────────────────────────────────────────────────────────
-- 15. SalePayment — Pagos asociados a una venta
--     Una venta puede pagarse con múltiples métodos (ej: parte
--     efectivo + parte tarjeta).
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE SalePayment (
    SalePaymentId       BIGINT          IDENTITY(1,1) PRIMARY KEY,
    SaleId              BIGINT          NOT NULL REFERENCES Sale(SaleId),
    PaymentMethodId     BIGINT          NOT NULL REFERENCES PaymentMethod(PaymentMethodId),
    Amount              NUMERIC(18,2)   NOT NULL,   -- Monto pagado con este método
    Reference           VARCHAR(100)    NULL         -- Nro. de autorización de tarjeta, etc.
);

-- ────────────────────────────────────────────────────────────────────
-- 16. Credit — Crédito (F2 Créditos)
--     Cuando el cliente no paga completo en el momento, se genera
--     un crédito. El saldo se abona con CreditPayment.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Credit (
    CreditId            BIGINT          IDENTITY(1,1) PRIMARY KEY,
    ClientId            BIGINT          NOT NULL REFERENCES Client(ClientId),
    SaleId              BIGINT          NULL REFERENCES Sale(SaleId),   -- NULL = crédito manual
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    OriginalAmount      NUMERIC(18,2)   NOT NULL,   -- Monto original del crédito
    Balance             NUMERIC(18,2)   NOT NULL,   -- Saldo pendiente de pagar
    DueDate             DATETIME        NULL,        -- Fecha límite de pago (opcional)
    Notes               VARCHAR(500)    NULL,
    Active              TINYINT         NOT NULL DEFAULT 1,  -- 0 = saldado
    CreatedAt           DATETIME        NOT NULL DEFAULT GETDATE()
);

-- ────────────────────────────────────────────────────────────────────
-- 17. CreditPayment — Abonos al crédito
--     Registra cada pago parcial que hace el cliente para saldar
--     su cuenta de crédito.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE CreditPayment (
    CreditPaymentId     BIGINT          IDENTITY(1,1) PRIMARY KEY,
    CreditId            BIGINT          NOT NULL REFERENCES Credit(CreditId),
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    PaymentMethodId     BIGINT          NOT NULL REFERENCES PaymentMethod(PaymentMethodId),
    Amount              NUMERIC(18,2)   NOT NULL,
    PaymentDate         DATETIME        NOT NULL DEFAULT GETDATE(),
    Notes               VARCHAR(300)    NULL
);

-- ────────────────────────────────────────────────────────────────────
-- 18. Purchase — Compra a proveedor (módulo Compras)
--     Registra la recepción de mercancía. Al confirmarla,
--     el stock de los productos asociados aumenta automáticamente.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Purchase (
    PurchaseId          BIGINT          IDENTITY(1,1) PRIMARY KEY,
    StoreId             BIGINT          NOT NULL REFERENCES Store(StoreId),
    SupplierId          BIGINT          NOT NULL REFERENCES Supplier(SupplierId),
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    SupplierInvoice     VARCHAR(50)     NULL,           -- Folio de la factura del proveedor
    PurchaseDate        DATETIME        NOT NULL,
    Subtotal            NUMERIC(18,2)   NOT NULL DEFAULT 0,
    TaxAmount           NUMERIC(18,2)   NOT NULL DEFAULT 0,
    Total               NUMERIC(18,2)   NOT NULL DEFAULT 0,
    Notes               VARCHAR(MAX)    NULL,
    IsConfirmed         TINYINT         NOT NULL DEFAULT 0,
                                                         -- 1 = mercancía recibida y stock actualizado
    CreatedAt           DATETIME        NOT NULL
);

-- ────────────────────────────────────────────────────────────────────
-- 19. PurchaseDetail — Renglones de la compra
--     Qué productos se compraron, en qué cantidad y a qué costo.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE PurchaseDetail (
    PurchaseDetailId    BIGINT          IDENTITY(1,1) PRIMARY KEY,
    PurchaseId          BIGINT          NOT NULL REFERENCES Purchase(PurchaseId),
    ProductId           BIGINT          NOT NULL REFERENCES Product(ProductId),
    Quantity            NUMERIC(18,2)   NOT NULL,
    UnitCost            NUMERIC(18,2)   NOT NULL,   -- Costo unitario pactado con el proveedor
    Subtotal            NUMERIC(18,2)   NOT NULL    -- Quantity * UnitCost
);

-- ────────────────────────────────────────────────────────────────────
-- 20. InventoryMovement — Movimiento de inventario (F7/F8)
--     Registra cualquier ajuste manual de stock: entradas (F7),
--     salidas (F8), mermas, ajustes de inventario físico.
--     Las ventas y compras también pueden generar movimientos aquí
--     como bitácora.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE InventoryMovement (
    InventoryMovementId BIGINT          IDENTITY(1,1) PRIMARY KEY,
    ProductId           BIGINT          NOT NULL REFERENCES Product(ProductId),
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    SupplierId          BIGINT          NULL REFERENCES Supplier(SupplierId),
                                                         -- Proveedor origen (solo en entradas)
    SaleId              BIGINT          NULL REFERENCES Sale(SaleId),
                                                         -- Venta origen (salidas por venta)
    PurchaseId          BIGINT          NULL REFERENCES Purchase(PurchaseId),
                                                         -- Compra origen (entradas por compra)
    MovementType        VARCHAR(10)     NOT NULL,        -- 'IN' | 'OUT' | 'ADJUST'
    Quantity            NUMERIC(18,2)   NOT NULL,        -- Positivo = entrada, negativo = salida
    StockBefore         NUMERIC(18,2)   NOT NULL,        -- Existencia antes del movimiento
    StockAfter          NUMERIC(18,2)   NOT NULL,        -- Existencia después del movimiento
    UnitCost            NUMERIC(18,2)   NULL,             -- Costo unitario (relevante en entradas)
    Reference           VARCHAR(100)    NULL,             -- Referencia externa (factura, nota)
    Reason              VARCHAR(300)    NULL,             -- Motivo del ajuste manual
    MovementDate        DATETIME        NOT NULL DEFAULT GETDATE()
);

-- ────────────────────────────────────────────────────────────────────
-- 21. CashCut — Corte de caja (módulo Corte)
--     Registro del cierre de turno del cajero. Compara el dinero
--     esperado según ventas vs el dinero contado físicamente.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE CashCut (
    CashCutId           BIGINT          IDENTITY(1,1) PRIMARY KEY,
    StoreId             BIGINT          NOT NULL REFERENCES Store(StoreId),
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    OpeningAmount       NUMERIC(18,2)   NOT NULL DEFAULT 0,
                                                         -- Fondo inicial puesto al abrir caja
    ExpectedAmount      NUMERIC(18,2)   NOT NULL DEFAULT 0,
                                                         -- Total calculado por el sistema
    ActualAmount        NUMERIC(18,2)   NOT NULL DEFAULT 0,
                                                         -- Total contado físicamente
    Difference          NUMERIC(18,2)   NOT NULL DEFAULT 0,
                                                         -- ActualAmount - ExpectedAmount (puede ser negativo)
    TotalSales          INT             NOT NULL DEFAULT 0,  -- Número de ventas en el turno
    OpenedAt            DATETIME        NOT NULL,            -- Hora de apertura de caja
    ClosedAt            DATETIME        NULL,                -- Hora de cierre
    Notes               VARCHAR(500)    NULL
);

-- ────────────────────────────────────────────────────────────────────
-- 22. CashCutDetail — Desglose del corte por método de pago
--     Muestra cuánto se recaudó en efectivo, cuánto en tarjeta, etc.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE CashCutDetail (
    CashCutDetailId     BIGINT          IDENTITY(1,1) PRIMARY KEY,
    CashCutId           BIGINT          NOT NULL REFERENCES CashCut(CashCutId),
    PaymentMethodId     BIGINT          NOT NULL REFERENCES PaymentMethod(PaymentMethodId),
    TotalCollected      NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Total recaudado con este método
    TransactionCount    INT             NOT NULL DEFAULT 0   -- Número de transacciones
);

-- ────────────────────────────────────────────────────────────────────
-- 23. Invoice — Factura fiscal (módulo Facturas)
--     Documento fiscal emitido a partir de una venta.
--     Puede representar CFDI (MX), factura electrónica, etc.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Invoice (
    InvoiceId           BIGINT          IDENTITY(1,1) PRIMARY KEY,
    SaleId              BIGINT          NOT NULL REFERENCES Sale(SaleId),
    ClientId            BIGINT          NOT NULL REFERENCES Client(ClientId),
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    InvoiceNumber       VARCHAR(50)     NOT NULL UNIQUE,  -- Folio fiscal
    UUID                VARCHAR(50)     NULL,              -- UUID del CFDI (si aplica)
    InvoiceDate         DATETIME        NOT NULL DEFAULT GETDATE(),
    Subtotal            NUMERIC(18,2)   NOT NULL,
    TaxAmount           NUMERIC(18,2)   NOT NULL,
    Total               NUMERIC(18,2)   NOT NULL,
    Status              VARCHAR(20)     NOT NULL DEFAULT 'active',
                                                           -- 'active' | 'cancelled'
    XmlContent          VARCHAR(MAX)    NULL,              -- XML del comprobante digital
    CreatedAt           DATETIME        NOT NULL DEFAULT GETDATE()
);

-- ────────────────────────────────────────────────────────────────────
-- 24. ReturnSale — Devolución (Ventas del día y Devoluciones)
--     Cuando un cliente regresa un producto ya vendido.
--     El stock se restaura y puede generarse un reembolso.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE ReturnSale (
    ReturnSaleId        BIGINT          IDENTITY(1,1) PRIMARY KEY,
    SaleId              BIGINT          NOT NULL REFERENCES Sale(SaleId),
    StoreId             BIGINT          NOT NULL REFERENCES Store(StoreId),
    SystemUserId        BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    ReturnDate          DATETIME        NOT NULL,
    Reason              VARCHAR(500)    NULL,
    Total               NUMERIC(18,2)   NOT NULL DEFAULT 0,
    CreatedAt           DATETIME        NOT NULL
);

-- ────────────────────────────────────────────────────────────────────
-- 25. ReturnSaleDetail — Renglones de la devolución
--     Qué artículo(s) se devuelven y en qué cantidad.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE ReturnSaleDetail (
    ReturnSaleDetailId  BIGINT          IDENTITY(1,1) PRIMARY KEY,
    ReturnSaleId        BIGINT          NOT NULL REFERENCES ReturnSale(ReturnSaleId),
    ProductId           BIGINT          NOT NULL REFERENCES Product(ProductId),
    ProductNameSnapshot VARCHAR(200)    NOT NULL,
    Quantity            NUMERIC(18,2)   NOT NULL,
    UnitPrice           NUMERIC(18,2)   NOT NULL,
    Subtotal            NUMERIC(18,2)   NOT NULL
);

-- ────────────────────────────────────────────────────────────────────
-- 26. Config — Configuración del sistema
--     Tabla clave-valor para parámetros del POS sin necesidad
--     de redesplegar: nombre del ticket, impuesto por defecto,
--     formato de folio, etc.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Config (
    ConfigId        INT             IDENTITY(1,1) PRIMARY KEY,
    StoreId         BIGINT          NULL REFERENCES Store(StoreId),
                                    -- NULL = configuración global; con valor = solo esa tienda
    [Key]           VARCHAR(100)    NOT NULL,   -- Nombre del parámetro ("default_tax_rate")
    Value           VARCHAR(500)    NOT NULL,   -- Valor del parámetro ("16.00")
    Description     VARCHAR(300)    NULL,        -- Explicación para el administrador
    CONSTRAINT UQ_Config UNIQUE (StoreId, [Key])
);

-- ====================================================================
-- DATOS INICIALES (seed)
-- ====================================================================

INSERT INTO SaleStatus (Name) VALUES ('Completada'), ('Pendiente'), ('Cancelada');

INSERT INTO PaymentMethod (Name, RequiresRef, IsCash) VALUES
    ('Efectivo',        0, 1),   -- IsCash=1: cuenta para el monto esperado en corte de caja
    ('Tarjeta débito',  1, 0),
    ('Tarjeta crédito', 1, 0),
    ('Transferencia',   1, 0),
    ('Vale / Cupón',    1, 0);

INSERT INTO PriceList (Name, Description) VALUES
    ('Precio Público', 'Lista estándar para ventas al detalle'),
    ('Mayoreo',        'Precios para compras al por mayor (F11)');

-- Nota: Unit es multi-tenant (StoreId NOT NULL) desde 2026-07-22.
-- Las 6 unidades por defecto (Pieza/kg/Litro/Metro/Caja/Paquete) se
-- crean automáticamente con StoreId = <storeId> al registrar una nueva
-- empresa en /crear-cuenta o al dar de alta una sucursal desde
-- SucursalesModule. No se insertan aquí como seed global.

-- Configuración SMTP global (StoreId = NULL — se aplica a toda la instancia)
INSERT INTO Config ([Key], Value, Description) VALUES
    ('smtp_test_mode',       '1',      '1 = guardar correos en wwwroot/email-debug/ sin enviarlos (desarrollo); 0 = envío real'),
    ('smtp_host',            '',       'Servidor SMTP (ej: smtp.gmail.com, smtp.sendgrid.net)'),
    ('smtp_port',            '587',    'Puerto SMTP: 587 = STARTTLS (recomendado), 465 = SSL directo, 25 = sin cifrado'),
    ('smtp_ssl',             '0',      '1 = SSL directo en puerto 465; 0 = STARTTLS en puerto 587'),
    ('smtp_user',            '',       'Email o usuario de la cuenta SMTP'),
    ('smtp_password',        '',       'Contraseña o App Password de la cuenta SMTP'),
    ('smtp_from_email',      '',       'Dirección que aparece como remitente (puede ser distinta al usuario SMTP)'),
    ('smtp_from_name',       'FiyiPOS',  'Nombre visible del remitente en el cliente de correo del destinatario');

-- Nota: las configuraciones del POS (default_tax_rate, ticket_footer, folio_prefix,
-- folio_padding, allow_negative_stock, require_customer_sale, currency_symbol)
-- se crean automáticamente con StoreId = <storeId> al registrar una nueva empresa
-- en /crear-cuenta (CreateAccount.razor → configRepository.AddRangeAsync).
-- No se insertan aquí como seed global.

-- ────────────────────────────────────────────────────────────────────
-- Migraciones sobre una base de datos existente (ejecutar una sola vez)
-- ────────────────────────────────────────────────────────────────────

-- Agregado para el flujo de registro público:
-- ALTER TABLE SystemUser ADD ActivationToken VARCHAR(50) NULL;
-- ALTER TABLE SystemUser ALTER COLUMN Username VARCHAR(200) NOT NULL;

-- Agregado para el flujo de "Olvidé mi contraseña" (2026-08-14):
-- ALTER TABLE SystemUser ADD PasswordResetToken VARCHAR(50) NULL;
-- ALTER TABLE SystemUser ADD PasswordResetTokenExpiresAt DATETIME NULL;

-- Eliminación de SystemUser.StoreId (reemplazado por SystemUserStore N:M):
-- DECLARE @sql NVARCHAR(500)
-- SELECT @sql = 'ALTER TABLE SystemUser DROP CONSTRAINT ' + name
-- FROM sys.foreign_keys
-- WHERE parent_object_id  = OBJECT_ID('dbo.SystemUser')
--   AND referenced_object_id = OBJECT_ID('dbo.Store')
-- EXEC sp_executesql @sql
-- ALTER TABLE SystemUser DROP COLUMN StoreId;

-- Suscripcion mensual (2026-05-11):
-- ALTER TABLE Store ADD BillingDueDate DATETIME NULL;
-- UPDATE Store SET BillingDueDate = DATEADD(DAY, 30, GETDATE()) WHERE BillingDueDate IS NULL;

-- Devoluciones (2026-05-11): crear tablas en BD existente
-- CREATE TABLE ReturnSale (
--     ReturnSaleId  BIGINT IDENTITY(1,1) PRIMARY KEY,
--     SaleId        BIGINT NOT NULL REFERENCES Sale(SaleId),
--     StoreId       BIGINT NOT NULL REFERENCES Store(StoreId),
--     SystemUserId  BIGINT NOT NULL REFERENCES SystemUser(SystemUserId),
--     ReturnDate    DATETIME NOT NULL,
--     Reason        VARCHAR(500) NULL,
--     Total         NUMERIC(18,2) NOT NULL DEFAULT 0,
--     CreatedAt     DATETIME NOT NULL
-- );
-- CREATE TABLE ReturnSaleDetail (
--     ReturnSaleDetailId BIGINT IDENTITY(1,1) PRIMARY KEY,
--     ReturnSaleId       BIGINT NOT NULL REFERENCES ReturnSale(ReturnSaleId),
--     ProductId          BIGINT NOT NULL REFERENCES Product(ProductId),
--     ProductNameSnapshot VARCHAR(200) NOT NULL,
--     Quantity           NUMERIC(18,2) NOT NULL,
--     UnitPrice          NUMERIC(18,2) NOT NULL,
--     Subtotal           NUMERIC(18,2) NOT NULL
-- );

-- Scheme Manager (2026-06-28): seed de variables CSS por sucursal en BD existente
-- Ejecutar una sola vez. Inserta las vars de colores para cada Store que aún no las tenga.
-- El diseño es POR SUCURSAL (StoreId NOT NULL): cada admin tiene su propio esquema de colores.
-- Las nuevas sucursales reciben el seed automáticamente al crearlas desde SucursalesModule.
INSERT INTO Config (StoreId, [Key], Value, Description)
SELECT s.StoreId, v.[Key], v.Value, v.Description
FROM Store s
CROSS JOIN (VALUES
    ('pos_bg',            '#f0faf5', 'Fondo general del área de contenido POS'),
    ('pos_surface',       '#ffffff',  'Fondo de cards, tablas y modales'),
    ('pos_primary',       '#1aacb1',  'Color primario: botones, links, badges'),
    ('pos_primary_dark',  '#148a8e',  'Color primario oscuro: hover y gradientes'),
    ('pos_primary_light', '#22c9ce',  'Color primario claro'),
    ('pos_primary_muted', '#d4f4f5',  'Color primario muy suave: chips y badges'),
    ('pos_text',          '#3A4656',  'Texto principal'),
    ('pos_text_muted',    '#64748b',  'Texto secundario / gris'),
    ('pos_border',        '#e2e8f0',  'Bordes de inputs, tablas y divisores'),
    ('pos_success',       '#4CAF50',  'Verde: activo, confirmado'),
    ('pos_success_dark',  '#3d8b40',  'Verde oscuro: hover de éxito'),
    ('pos_warning',       '#f59e0b',  'Amarillo: alerta, pendiente'),
    ('pos_danger',        '#ef4444',  'Rojo: error, inactivo')
) AS v([Key], Value, Description)
WHERE NOT EXISTS (
    SELECT 1 FROM Config c WHERE c.StoreId = s.StoreId AND c.[Key] = v.[Key]
);

-- Notificaciones por email por sucursal (2026-07-04): flags que aparecen en ConfiguracionModule
-- Ejecutar una sola vez para sucursales ya existentes.
INSERT INTO Config (StoreId, [Key], Value, Description)
SELECT s.StoreId, v.[Key], v.Value, v.Description
FROM Store s
CROSS JOIN (VALUES
    ('send_email_to_cashier_when_araise_low_stock',                           '0', 'Enviar email a cajeros cuando hay stock bajo'),
    ('send_email_to_supervisor_when_araise_low_stock',                        '0', 'Enviar email a supervisores cuando hay stock bajo'),
    ('send_email_to_admin_when_araise_low_stock',                             '0', 'Enviar email a admins cuando hay stock bajo'),
    ('send_email_to_admin_and_supervisor_when_a_cash_cut_is_not_exact_block', '0', 'Enviar email a admins y supervisores cuando un corte de caja no cuadra')
) AS v([Key], Value, Description)
WHERE NOT EXISTS (
    SELECT 1 FROM Config c WHERE c.StoreId = s.StoreId AND c.[Key] = v.[Key]
);

-- Promociones (2026-05-23): combos de productos a precio especial
-- CREATE TABLE Promotion (
--     PromotionId  BIGINT IDENTITY(1,1) PRIMARY KEY,
--     StoreId      BIGINT NOT NULL REFERENCES Store(StoreId),
--     Name         VARCHAR(150) NOT NULL,
--     Description  VARCHAR(500) NULL,
--     Price        NUMERIC(18,2) NOT NULL,
--     Active       BIT NOT NULL DEFAULT 1,
--     CreatedAt    DATETIME NOT NULL DEFAULT GETDATE()
-- );
-- CREATE TABLE PromotionDetail (
--     PromotionDetailId   BIGINT IDENTITY(1,1) PRIMARY KEY,
--     PromotionId         BIGINT NOT NULL REFERENCES Promotion(PromotionId),
--     ProductId           BIGINT NOT NULL REFERENCES Product(ProductId),
--     ProductNameSnapshot VARCHAR(150) NOT NULL,
--     Quantity            NUMERIC(18,2) NOT NULL
-- );
-- ALTER TABLE SaleDetail ADD PromotionId BIGINT NULL;
-- ALTER TABLE SaleDetail ALTER COLUMN ProductId BIGINT NULL;  -- combos use NULL for ProductId
--
-- Ejemplo de datos de prueba (reemplazar StoreId y ProductId según tu BD):
-- INSERT INTO Promotion (StoreId, Name, Description, Price, Active, CreatedAt)
--     VALUES (1, 'Combo Desayuno', '2 productos al precio especial', 25.00, 1, GETDATE());
-- INSERT INTO PromotionDetail (PromotionId, ProductId, ProductNameSnapshot, Quantity)
--     VALUES (1, 1, 'Producto A', 1), (1, 2, 'Producto B', 2);

-- ────────────────────────────────────────────────────────────────────
-- Notepad — Bloc de notas por sucursal (2026-06-28)
--   Una fila por tienda. Visible solo para supervisor y administrador.
--   4 bloques de texto libre independientes.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Notepad (
    NotepadId  BIGINT        IDENTITY(1,1) PRIMARY KEY,
    StoreId    BIGINT        NOT NULL REFERENCES Store(StoreId),
    Note1      VARCHAR(MAX)  NULL,
    Note2      VARCHAR(MAX)  NULL,
    Note3      VARCHAR(MAX)  NULL,
    Note4      VARCHAR(MAX)  NULL,
    CONSTRAINT UQ_Notepad_StoreId UNIQUE (StoreId)
);

-- ────────────────────────────────────────────────────────────────────
-- Attendance — Registro de entradas y salidas de operadores (2026-06-30)
--   CheckIn se registra al hacer Login, CheckOut al presionar Salir.
--   CheckOut NULL indica que el operador sigue en línea.
--   Visible solo para administrador.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE Attendance (
    AttendanceId  BIGINT    IDENTITY(1,1) PRIMARY KEY,
    SystemUserId  BIGINT    NOT NULL REFERENCES SystemUser(SystemUserId),
    StoreId       BIGINT    NOT NULL REFERENCES Store(StoreId),
    CheckIn       DATETIME  NOT NULL DEFAULT GETDATE(),
    CheckOut      DATETIME  NULL
);

-- ────────────────────────────────────────────────────────────────────
-- InternalConsumption — Consumo interno (módulo Ventas > Consumo interno, 2026-07-04)
--   Registra productos que la propia tienda consume (personal, muestras,
--   mermas, preparación) en vez de vender. Disponible para cashier,
--   supervisor y admin. Cada confirmación descuenta stock y genera un
--   InventoryMovement de tipo 'OUT' con Reference 'CI-<InternalConsumptionId>'.
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE InternalConsumption (
    InternalConsumptionId BIGINT          IDENTITY(1,1) PRIMARY KEY,
    StoreId                BIGINT          NOT NULL REFERENCES Store(StoreId),
    SystemUserId           BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
    ConsumptionDate        DATETIME        NOT NULL DEFAULT GETDATE(),
    Reason                 VARCHAR(300)    NOT NULL,        -- Motivo del consumo (ej: "Consumo del personal")
    TotalCost              NUMERIC(18,2)   NOT NULL DEFAULT 0,  -- Suma de los subtotales a costo
    CreatedAt              DATETIME        NOT NULL DEFAULT GETDATE()
);

-- ────────────────────────────────────────────────────────────────────
-- InternalConsumptionDetail — Renglones del consumo interno
--   Qué producto(s) se consumieron, en qué cantidad y a qué costo
--   unitario (snapshot de Product.CostPrice al momento del consumo).
-- ────────────────────────────────────────────────────────────────────
CREATE TABLE InternalConsumptionDetail (
    InternalConsumptionDetailId BIGINT          IDENTITY(1,1) PRIMARY KEY,
    InternalConsumptionId       BIGINT          NOT NULL REFERENCES InternalConsumption(InternalConsumptionId),
    ProductId                   BIGINT          NOT NULL REFERENCES Product(ProductId),
    ProductNameSnapshot         VARCHAR(200)    NOT NULL,
    Quantity                    NUMERIC(18,2)   NOT NULL,
    UnitCost                    NUMERIC(18,2)   NOT NULL,
    Subtotal                    NUMERIC(18,2)   NOT NULL   -- Quantity * UnitCost
);

-- ====================================================================
-- MIGRACIÓN (2026-07-06): Renombrado de tablas por grupo de dominio
--   Convención: FiyiPOS.<Grupo>.<Tabla> según la carpeta del backend:
--     · Common       → toda variante (Físico, Online, Restó, Canchas)
--     · Catalogo     → variantes con productos y stock
--     · PuntoDeVenta → flujos exclusivos del mostrador físico
--   El nombre contiene puntos: SQL Server lo permite; siempre debe
--   referenciarse entre corchetes, ej: SELECT * FROM [FiyiPOS.Common.Store]
--   Script idempotente: solo renombra si la tabla vieja existe y la
--   nueva no. Corre igual en BD existente o tras un install desde cero.
-- ====================================================================

-- ── Common ──────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.Store','U')            IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.Store]','U')            IS NULL EXEC sp_rename 'dbo.Store',            'FiyiPOS.Common.Store';
IF OBJECT_ID('dbo.SystemUser','U')       IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.SystemUser]','U')       IS NULL EXEC sp_rename 'dbo.SystemUser',       'FiyiPOS.Common.SystemUser';
IF OBJECT_ID('dbo.SystemUserStore','U')  IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.SystemUserStore]','U')  IS NULL EXEC sp_rename 'dbo.SystemUserStore',  'FiyiPOS.Common.SystemUserStore';
IF OBJECT_ID('dbo.Client','U')           IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.Client]','U')           IS NULL EXEC sp_rename 'dbo.Client',           'FiyiPOS.Common.Client';
IF OBJECT_ID('dbo.PaymentMethod','U')    IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.PaymentMethod]','U')    IS NULL EXEC sp_rename 'dbo.PaymentMethod',    'FiyiPOS.Common.PaymentMethod';
IF OBJECT_ID('dbo.SaleStatus','U')       IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.SaleStatus]','U')       IS NULL EXEC sp_rename 'dbo.SaleStatus',       'FiyiPOS.Common.SaleStatus';
IF OBJECT_ID('dbo.Sale','U')             IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.Sale]','U')             IS NULL EXEC sp_rename 'dbo.Sale',             'FiyiPOS.Common.Sale';
IF OBJECT_ID('dbo.SaleDetail','U')       IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.SaleDetail]','U')       IS NULL EXEC sp_rename 'dbo.SaleDetail',       'FiyiPOS.Common.SaleDetail';
IF OBJECT_ID('dbo.SalePayment','U')      IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.SalePayment]','U')      IS NULL EXEC sp_rename 'dbo.SalePayment',      'FiyiPOS.Common.SalePayment';
IF OBJECT_ID('dbo.CashCut','U')          IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.CashCut]','U')          IS NULL EXEC sp_rename 'dbo.CashCut',          'FiyiPOS.Common.CashCut';
IF OBJECT_ID('dbo.CashCutDetail','U')    IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.CashCutDetail]','U')    IS NULL EXEC sp_rename 'dbo.CashCutDetail',    'FiyiPOS.Common.CashCutDetail';
IF OBJECT_ID('dbo.CashMovement','U')     IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.CashMovement]','U')     IS NULL EXEC sp_rename 'dbo.CashMovement',     'FiyiPOS.Common.CashMovement';
IF OBJECT_ID('dbo.Config','U')           IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.Config]','U')           IS NULL EXEC sp_rename 'dbo.Config',           'FiyiPOS.Common.Config';
IF OBJECT_ID('dbo.CompanySettings','U')  IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.CompanySettings]','U')  IS NULL EXEC sp_rename 'dbo.CompanySettings',  'FiyiPOS.Common.CompanySettings';
IF OBJECT_ID('dbo.Notepad','U')          IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.Notepad]','U')          IS NULL EXEC sp_rename 'dbo.Notepad',          'FiyiPOS.Common.Notepad';
IF OBJECT_ID('dbo.Attendance','U')       IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.Attendance]','U')       IS NULL EXEC sp_rename 'dbo.Attendance',       'FiyiPOS.Common.Attendance';
IF OBJECT_ID('dbo.Invoice','U')          IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.Invoice]','U')          IS NULL EXEC sp_rename 'dbo.Invoice',          'FiyiPOS.Common.Invoice';
IF OBJECT_ID('dbo.AfipAccessTicket','U') IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.AfipAccessTicket]','U') IS NULL EXEC sp_rename 'dbo.AfipAccessTicket', 'FiyiPOS.Common.AfipAccessTicket';
IF OBJECT_ID('dbo.AfipLog','U')          IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Common.AfipLog]','U')          IS NULL EXEC sp_rename 'dbo.AfipLog',          'FiyiPOS.Common.AfipLog';

-- ── Catalogo ────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.Category','U')                  IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.Category]','U')                  IS NULL EXEC sp_rename 'dbo.Category',                  'FiyiPOS.Catalogo.Category';
IF OBJECT_ID('dbo.Unit','U')                      IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.Unit]','U')                      IS NULL EXEC sp_rename 'dbo.Unit',                      'FiyiPOS.Catalogo.Unit';
IF OBJECT_ID('dbo.Product','U')                   IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.Product]','U')                   IS NULL EXEC sp_rename 'dbo.Product',                   'FiyiPOS.Catalogo.Product';
IF OBJECT_ID('dbo.Promotion','U')                 IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.Promotion]','U')                 IS NULL EXEC sp_rename 'dbo.Promotion',                 'FiyiPOS.Catalogo.Promotion';
IF OBJECT_ID('dbo.PromotionDetail','U')           IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.PromotionDetail]','U')           IS NULL EXEC sp_rename 'dbo.PromotionDetail',           'FiyiPOS.Catalogo.PromotionDetail';
IF OBJECT_ID('dbo.Supplier','U')                  IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.Supplier]','U')                  IS NULL EXEC sp_rename 'dbo.Supplier',                  'FiyiPOS.Catalogo.Supplier';
IF OBJECT_ID('dbo.Purchase','U')                  IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.Purchase]','U')                  IS NULL EXEC sp_rename 'dbo.Purchase',                  'FiyiPOS.Catalogo.Purchase';
IF OBJECT_ID('dbo.PurchaseDetail','U')            IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.PurchaseDetail]','U')            IS NULL EXEC sp_rename 'dbo.PurchaseDetail',            'FiyiPOS.Catalogo.PurchaseDetail';
IF OBJECT_ID('dbo.InventoryMovement','U')         IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.InventoryMovement]','U')         IS NULL EXEC sp_rename 'dbo.InventoryMovement',         'FiyiPOS.Catalogo.InventoryMovement';
IF OBJECT_ID('dbo.InternalConsumption','U')       IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.InternalConsumption]','U')       IS NULL EXEC sp_rename 'dbo.InternalConsumption',       'FiyiPOS.Catalogo.InternalConsumption';
IF OBJECT_ID('dbo.InternalConsumptionDetail','U') IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.Catalogo.InternalConsumptionDetail]','U') IS NULL EXEC sp_rename 'dbo.InternalConsumptionDetail', 'FiyiPOS.Catalogo.InternalConsumptionDetail';

-- ── PuntoDeVenta ────────────────────────────────────────────────────
IF OBJECT_ID('dbo.Credit','U')           IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.PuntoDeVenta.Credit]','U')           IS NULL EXEC sp_rename 'dbo.Credit',           'FiyiPOS.PuntoDeVenta.Credit';
IF OBJECT_ID('dbo.CreditPayment','U')    IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.PuntoDeVenta.CreditPayment]','U')    IS NULL EXEC sp_rename 'dbo.CreditPayment',    'FiyiPOS.PuntoDeVenta.CreditPayment';
IF OBJECT_ID('dbo.ReturnSale','U')       IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.PuntoDeVenta.ReturnSale]','U')       IS NULL EXEC sp_rename 'dbo.ReturnSale',       'FiyiPOS.PuntoDeVenta.ReturnSale';
IF OBJECT_ID('dbo.ReturnSaleDetail','U') IS NOT NULL AND OBJECT_ID('dbo.[FiyiPOS.PuntoDeVenta.ReturnSaleDetail]','U') IS NULL EXEC sp_rename 'dbo.ReturnSaleDetail', 'FiyiPOS.PuntoDeVenta.ReturnSaleDetail';

-- ====================================================================
-- MIGRACIÓN (2026-07-07): Product.IsAvailableToSellOnline
--   Marca si el producto puede venderse en FiyiPOS Online.
--   Estrategia en 3 pasos (segura para tablas con datos):
--     1) Agregar la columna como NULL
--     2) Backfill: todos los productos existentes en 0 (falso)
--     3) Volverla NOT NULL + default 0 para inserts futuros
--   Idempotente: puede re-ejecutarse sin error.
-- ====================================================================

-- Paso 1: agregar columna como NULL
IF COL_LENGTH('dbo.[FiyiPOS.Catalogo.Product]', 'IsAvailableToSellOnline') IS NULL
    ALTER TABLE dbo.[FiyiPOS.Catalogo.Product] ADD IsAvailableToSellOnline TINYINT NULL;
GO

-- Paso 2: backfill de productos existentes en falso
UPDATE dbo.[FiyiPOS.Catalogo.Product]
SET IsAvailableToSellOnline = 0
WHERE IsAvailableToSellOnline IS NULL;
GO

-- Paso 3: volver la columna NOT NULL y con default 0
ALTER TABLE dbo.[FiyiPOS.Catalogo.Product]
ALTER COLUMN IsAvailableToSellOnline TINYINT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_Product_IsAvailableToSellOnline')
    ALTER TABLE dbo.[FiyiPOS.Catalogo.Product]
    ADD CONSTRAINT DF_Product_IsAvailableToSellOnline DEFAULT 0 FOR IsAvailableToSellOnline;
GO

-- ====================================================================
-- MIGRACIÓN (2026-07-07): Product.FirstImageURL
--   URL relativa de la primera imagen del producto para venta en línea
--   (ej: /uploads/product/3f2a8c1e-....png).
--   Acepta NULL porque los productos existentes no tienen imagen.
--   Idempotente: puede re-ejecutarse sin error.
-- ====================================================================
IF COL_LENGTH('dbo.[FiyiPOS.Catalogo.Product]', 'FirstImageURL') IS NULL
    ALTER TABLE dbo.[FiyiPOS.Catalogo.Product] ADD FirstImageURL VARCHAR(500) NULL;
GO

-- ====================================================================
-- MIGRACIÓN (2026-07-22): Unit se vuelve multi-tenant (StoreId)
--   Antes: Unit era una tabla global (Pieza/kg/Litro/Metro/Caja/Paquete)
--   compartida por todas las sucursales, sin CRUD para el usuario.
--   Ahora: cada Store administra sus propias unidades desde
--   Administrador → Unidades de medida (solo rol admin).
--
--   Estrategia (segura para tablas con datos, idempotente):
--     1) Agregar Unit.StoreId como NULL
--     2) Clonar cada unidad global (StoreId NULL) en cada Store existente
--     3) Remapear Product.UnitId de la unidad global a la copia de su
--        propia sucursal (mismo Name)
--     4) Borrar las unidades globales ya huérfanas (StoreId NULL)
--     5) Volver Unit.StoreId NOT NULL + FK a Store
--     6) Agregar Unit.Active (default 1, todas las unidades existentes
--        quedan activas)
--   Requisito: correr esta migración UNA sola vez, después de que ya
--   exista al menos una fila en Store.
-- ====================================================================

-- Paso 1: agregar StoreId como NULL
IF COL_LENGTH('dbo.[FiyiPOS.Catalogo.Unit]', 'StoreId') IS NULL
    ALTER TABLE dbo.[FiyiPOS.Catalogo.Unit] ADD StoreId BIGINT NULL;
GO

-- Paso 2: clonar cada unidad global (StoreId NULL) en cada sucursal que
-- todavía no tenga una unidad con ese mismo nombre.
INSERT INTO dbo.[FiyiPOS.Catalogo.Unit] (StoreId, Name, Abbreviation)
SELECT s.StoreId, u.Name, u.Abbreviation
FROM dbo.[FiyiPOS.Catalogo.Unit] u
CROSS JOIN dbo.[FiyiPOS.Common.Store] s
WHERE u.StoreId IS NULL
  AND NOT EXISTS (
      SELECT 1 FROM dbo.[FiyiPOS.Catalogo.Unit] u2
      WHERE u2.StoreId = s.StoreId AND u2.Name = u.Name
  );
GO

-- Paso 3: remapear Product.UnitId de la unidad global a la copia de su
-- propia sucursal (empareja por Name, ya clonado en el paso anterior).
UPDATE p
SET p.UnitId = u_new.UnitId
FROM dbo.[FiyiPOS.Catalogo.Product] p
JOIN dbo.[FiyiPOS.Catalogo.Unit] u_old ON u_old.UnitId = p.UnitId AND u_old.StoreId IS NULL
JOIN dbo.[FiyiPOS.Catalogo.Unit] u_new ON u_new.StoreId = p.StoreId AND u_new.Name = u_old.Name;
GO

-- Paso 4: borrar las unidades globales, ya sin productos que las referencien.
DELETE FROM dbo.[FiyiPOS.Catalogo.Unit] WHERE StoreId IS NULL;
GO

-- Paso 5: volver StoreId NOT NULL + agregar la FK a Store.
ALTER TABLE dbo.[FiyiPOS.Catalogo.Unit]
ALTER COLUMN StoreId BIGINT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Unit_Store')
    ALTER TABLE dbo.[FiyiPOS.Catalogo.Unit]
    ADD CONSTRAINT FK_Unit_Store FOREIGN KEY (StoreId) REFERENCES dbo.[FiyiPOS.Common.Store](StoreId);
GO

-- Paso 6: agregar Active (todas las unidades existentes quedan activas).
IF COL_LENGTH('dbo.[FiyiPOS.Catalogo.Unit]', 'Active') IS NULL
    ALTER TABLE dbo.[FiyiPOS.Catalogo.Unit] ADD Active TINYINT NOT NULL DEFAULT 1;
GO
