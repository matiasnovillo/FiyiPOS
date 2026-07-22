# FiyiPOS — Guías para Claude

## Generalidades

- La app tiene un modelo de programación basado en features: en `/Areas` se encuentran las carpetas de cada feature (ej: CMS, System, FiyiPOS) con sus archivos backend, y en `/Components/Pages` están las páginas Razor organizadas por módulo (`CMSPages`, `SystemPages`, `CUSTOMPAGES/FiyiPOSApp`, etc.).
- La app contiene los módulos: CMS + Login + Dashboard + FiyiPOS (razón principal de la app: generar un sistema web POS).
- No usar Code-First. Las tablas de la BD se crean y modifican mediante scripts SQL en `/Backup/pos_schema.sql`.
- Al agregar nuevas o modificar funcionalidades, encargate de actualizar el archivo GUIA_DE_USO.html

## Frontend

### CSS

Todos los CSS que construyas para esta app guárdalos en `wwwroot/css/FiyiPOS/FiyiPOS.css`.

No uses `<style>` inline en los archivos `.razor` ni crees archivos `.razor.css` separados. El único archivo de estilos para el POS es `wwwroot/css/FiyiPOS/FiyiPOS.css`, que ya está referenciado globalmente en `Components/Shared/ComponentsForAll/_HEADApp.razor`.

### Componentes del módulo FiyiPOS

- Página principal: `Components/Pages/CUSTOMPAGES/FiyiPOSApp/FiyiPOSApp.razor`
- Componentes compartidos del POS: `Components/Shared/ComponentsForFiyiPOS/` (`_Header.razor`, `_NavBar.razor`, `_StatusBar.razor`)

## Backend

- Tecnologías: .NET 10, Fluent API, Custom DataAnnotations, Blazor Server-Side, LINQ, EF Core
- Patrones de diseño: DI, Layered approach, KISS, YAGNI, SOLID

### Archivos del backend

`TableX` es un placeholder — reemplazarlo por el nombre real de la tabla (ej: `Product`, `Sale`, `Customer`).

- Database context: `/DatabaseContexts/FiyiPOSDbContext.cs`
- En `/Areas/TableXBack/` se encuentran los archivos CRUD para cada entidad, de más baja capa a más alta:
  - `/Entities/TableX.cs` — clase anémica con getters/setters y entidades de navegación. Usa Custom DataAnnotations cuyas configuraciones están en `/Library/ModelAttributeValidator/`.
  - `/EntitiesConfiguration/TableXConfiguration.cs` — mapeo de entidades a tablas SQL Server con Fluent API.
  - `/Repositories/TableXRepository.cs` — puente entre la aplicación y los datos.
  - `/Interfaces/ITableXRepository.cs` — interfaz para inyección de dependencias del repositorio.
  - `/DTOs/PaginatedTableXDTO.cs` — transferencia de datos paginados desde el repositorio a las páginas Razor.

## Base de datos

- Tecnología: Microsoft SQL Server
- Schema y migraciones: `/Backup/pos_schema.sql`
- No se usa Code-First ni migraciones automáticas de EF Core.
