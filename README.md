# 🛒 FiyiPOS

**Sistema web de punto de venta y gestión comercial, desarrollado con .NET 10, Blazor Server y SQL Server.**

FiyiPOS reúne la operación diaria de un comercio: vender, cobrar, controlar existencias, registrar compras y cerrar la caja. Su propósito es conectar esas tareas en una misma aplicación para que cajeros, supervisores y administradores puedan trabajar con información compartida y consultar los resultados de cada sucursal.

## 🎯 Propósito de la aplicación

El núcleo del producto es el **punto de venta presencial**. Una venta relaciona productos, cantidades, cliente y medios de pago; al confirmarse, se guardan sus detalles y se descuenta el stock dentro de una transacción de base de datos. Las compras, devoluciones y consumos internos complementan el seguimiento de las existencias.

La aplicación también ofrece herramientas para administrar el negocio: créditos de clientes, movimientos de efectivo, cortes de caja, operadores, sucursales, reportes y facturación electrónica. Incluye páginas públicas de presentación, registro, activación de cuenta y recuperación de contraseña.

## ✨ Funcionalidades principales

| Área | Qué permite gestionar |
| --- | --- |
| 🧾 Ventas | Registro de ventas, detalle de productos, pagos, tickets y devoluciones. |
| 📦 Catálogo e inventario | Productos, categorías, unidades de medida, existencias y movimientos de inventario. |
| 🚚 Abastecimiento | Proveedores, compras y sus detalles, además de consumos internos. |
| 🏷️ Precios y promociones | Promociones, combos y modalidades de precio mayorista y venta nocturna. |
| 👥 Clientes y créditos | Datos de clientes, ventas a crédito, saldos y registro de abonos. |
| 💵 Caja | Apertura con efectivo inicial, movimientos, cortes y reportes de cierre. |
| 🏪 Administración | Sucursales, operadores, asignación de tiendas y permisos por rol: cajero, supervisor y administrador. |
| 📊 Reportes | Consulta de resultados comerciales y exportación de productos, clientes y proveedores a Excel. |
| 🇦🇷 Facturación electrónica | Servicios AFIP de autenticación y emisión, certificados, comprobantes, registros de integración y códigos QR. Requiere configuración fiscal. |
| 🤖 Asistente de IA | Chat con contexto de la tienda mediante la API de Anthropic, sujeto a la configuración de su clave de acceso. |
| 🎨 Experiencia de uso | Vistas para escritorio y móvil, atajos de teclado y personalización visual. |

### 🚧 Alcance actual

El panel de control permite acceder a **FiyiPOS Físico** (`/punto-de-venta`) y **FiyiPOS Turnos** (`/turnos`). La ruta de turnos tiene una estructura de navegación que reutiliza módulos del POS; la presencia de esta pantalla no implica que exista una agenda completa de reservas implementada.

La organización de los datos permite compartir capacidades entre variantes del producto. Las menciones a otras variantes en comentarios del código expresan una dirección de expansión y no garantizan funcionalidades terminadas.

## 🛠️ Tecnologías

Las versiones indicadas corresponden a las referencias del proyecto en [FiyiPOS.csproj](FiyiPOS.csproj).

| Tecnología | Uso dentro de FiyiPOS |
| --- | --- |
| ⚙️ C# y ASP.NET Core / .NET 10 | Aplicación web, configuración de servicios y procesamiento del lado del servidor. |
| 🖥️ Blazor con Interactive Server | Interfaz basada en componentes Razor con interacción ejecutada en el servidor. |
| 🗄️ SQL Server y EF Core 10.0.5 | Persistencia relacional, consultas LINQ y operaciones asincrónicas. |
| 🧩 Fluent API y DataAnnotations personalizadas | Mapeo de entidades y validación de modelos. |
| 🔌 Inyección de dependencias y Scrutor | Registro de servicios y descubrimiento de repositorios y servicios de importación/exportación. |
| 🎨 Bootstrap 5, Font Awesome y JavaScript | Diseño adaptable, iconos e interoperabilidad para funciones del navegador. |
| 📗 ClosedXML y CsvHelper | Utilidades de importación/exportación de datos en Excel y CSV, según el módulo. |
| 📄 IronPDF y QRCoder | Generación de PDF en servicios de exportación y códigos QR para facturación. |
| ✉️ MailKit | Envío de correos mediante SMTP. |

El proyecto también referencia el SDK de Mercado Pago. Esta dependencia, por sí sola, no acredita un circuito automático de cobro integrado: la página de pago existente presenta instrucciones para abonar la suscripción.

## 🏗️ Arquitectura técnica

FiyiPOS se organiza como una **aplicación monolítica modular por funcionalidades**, con una separación por capas dentro de cada entidad. Los componentes Blazor utilizan repositorios y servicios mediante inyección de dependencias; estos acceden a SQL Server a través de Entity Framework Core.

```text
Componentes Blazor / páginas Razor
                 ↓
Repositorios y servicios de aplicación
                 ↓
FiyiPOSDbContext · EF Core · LINQ
                 ↓
             SQL Server
```

### 🧱 Organización del código

```text
Areas/
├── FiyiPOS/
│   ├── Common/          # Ventas, clientes, caja, sucursales y facturación
│   ├── Catalogo/        # Productos, compras, proveedores e inventario
│   └── PuntoDeVenta/    # Créditos, abonos y devoluciones
└── System/              # Registro de fallos y servicios asociados
Components/
├── Pages/               # Sitio público, cuentas y páginas de FiyiPOS
└── Shared/              # Componentes reutilizables de escritorio y móvil
DatabaseContexts/        # Contexto y configuración del modelo de datos
Services/                # Sesión POS, atajos, estado del sistema e integración AFIP
Library/                 # Validadores y utilidades compartidas
Mailing/                 # Envío de correos
Backup/pos_schema.sql    # Creación y evolución de la base de datos
wwwroot/                 # Recursos estáticos: CSS, JavaScript, imágenes e iconos
Program.cs               # Registro de dependencias y configuración HTTP
```

Dentro de cada carpeta `*Back`, la estructura habitual incluye:

- **`Entities/`**: modelos de datos y propiedades de navegación.
- **`EntitiesConfiguration/`**: configuración de tablas y relaciones mediante Fluent API.
- **`Repositories/` e `Interfaces/`**: acceso a datos y sus contratos.
- **`DTOs/`**: datos de intercambio, resultados paginados y modelos de consulta.
- **`Services/`**: tareas específicas, como exportaciones, cuando corresponde.

### 🔄 Persistencia y consistencia

El contexto [FiyiPOSDbContext](DatabaseContexts/FiyiPOSDbContext.cs) se obtiene mediante `IDbContextFactory`, lo que permite crear contextos por operación. Los flujos como ventas, compras, devoluciones y abonos utilizan transacciones explícitas para agrupar sus cambios.

Las sucursales y los operadores forman parte del modelo de negocio. `PosSessionService` mantiene el operador actual, su rol, la tienda seleccionada y las tiendas accesibles durante la sesión interactiva.

**La base de datos se administra mediante SQL:** [Backup/pos_schema.sql](Backup/pos_schema.sql) contiene las definiciones y cambios de estructura. No se utiliza Code-First ni migraciones automáticas de EF Core. El script reúne bloques de creación y de evolución; deben aplicarse según el estado de la base de destino.

### 🔐 Sesión y funcionamiento del servidor

La sesión se gestiona con servicios propios y restauración de datos desde cookies protegidas. Los permisos de navegación se definen por rol en `PosSessionService`.

[Program.cs](Program.cs) configura HTTPS, antiforgery, Data Protection y un límite global de 300 solicitudes por minuto por dirección IP. Un servicio en segundo plano verifica la conectividad con la base de datos y comunica su estado a la interfaz.

Al utilizar Blazor Interactive Server, la interacción requiere conexión activa con el servidor. El monitor de conectividad informa interrupciones; no representa un modo de venta sin conexión.

## 🚀 Ejecución local

### Requisitos

- SDK de **.NET 10**.
- Una instancia de **Microsoft SQL Server** y una base preparada con los bloques correspondientes de `Backup/pos_schema.sql`.
- Una cadena de conexión configurada con la clave `ConnectionStrings:DefaultConnection`, mediante configuración local o la variable de entorno `ConnectionStrings__DefaultConnection`.

### Iniciar la aplicación

Desde la raíz del repositorio:

```powershell
dotnet restore
dotnet build
dotnet run --launch-profile FiyiPOS3
```

El perfil de desarrollo está definido en [launchSettings.json](Properties/launchSettings.json) y utiliza `https://localhost:59919` y `http://localhost:59920`.

Las integraciones necesitan su propia configuración: SMTP para correos, certificados y parámetros fiscales para AFIP, y una clave de Anthropic para el asistente. Su disponibilidad depende de esas credenciales y de los servicios externos.

## 🧭 Puntos de entrada para desarrollar

- [Program.cs](Program.cs): servicios, conexión a SQL Server y middleware.
- [Panel de control](Components/Pages/FiyiPOSApp/Dashboard.razor): selección de sistema.
- [Punto de venta](Components/Pages/FiyiPOSApp/PuntoDeVenta.razor): composición y navegación del POS.
- [Componentes compartidos](Components/Shared/ComponentsForFiyiPOS/): navegación y elementos de escritorio y móvil.
- [Servicios AFIP](Services/AFIP/): autenticación, certificados y emisión electrónica.
- [FiyiPOS.css](wwwroot/css/FiyiPOS/FiyiPOS.css): archivo central de estilos del POS.
- [AGENTS.md](AGENTS.md): convenciones de organización y mantenimiento del proyecto.
