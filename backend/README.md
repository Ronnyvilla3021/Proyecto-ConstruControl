# ⚙️ ConstruControl — Backend

API REST construida en **ASP.NET Core / .NET 10**, siguiendo **Clean Architecture** en 4 capas. Incluye autenticación JWT, transacciones de negocio críticas, un motor de automatización que corre en segundo plano, y un hub de SignalR para actualizaciones en tiempo real.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Neon-4169E1?logo=postgresql)
![EF Core](https://img.shields.io/badge/EF%20Core-Code%20First-blueviolet)

---

## 🏗️ Arquitectura: Clean Architecture (4 capas)

```
backend/
├── ConstruControl.Domain/          → Entidades puras del negocio (Obra, Material, Compra...)
│                                       Sin dependencias externas, sin referencias a EF Core ni ASP.NET
├── ConstruControl.Application/     → Interfaces (contratos), DTOs, reglas de negocio de alto nivel
│                                       Depende solo de Domain
├── ConstruControl.Infrastructure/  → Implementaciones: EF Core, repositorios, JWT, storage, reportes
│                                       Depende de Application y Domain
└── ConstruControl.API/             → Controllers, SignalR, BackgroundService, configuración
                                        Depende de Application e Infrastructure
```

**Regla de dependencia:** las capas internas nunca conocen a las externas. `Domain` no sabe que existe una base de datos; `Application` define *qué* se necesita (una interfaz `IObraRepository`) sin saber *cómo* se implementa; `Infrastructure` es la única capa que sabe que la persistencia es PostgreSQL vía EF Core.

**Por qué importa en la práctica, no solo en la teoría:** este proyecto se desarrolló originalmente sobre **SQL Server**, y tuvo que migrarse a **PostgreSQL** para el despliegue (ver sección de Deploy). Gracias a esta separación, la migración fue: cambiar un paquete NuGet en `Infrastructure`, regenerar una migración de EF Core, y ajustar una cadena de conexión. **Cero cambios** en `Domain`, `Application`, o en ningún controller.

---

## 🛠️ Stack técnico

| Herramienta | Uso |
|---|---|
| ASP.NET Core / .NET 10 | Framework base, Web API con controllers |
| Entity Framework Core | ORM, Code First con Migrations |
| Npgsql | Proveedor de PostgreSQL para EF Core |
| JWT Bearer + BCrypt | Autenticación y hash de contraseñas |
| SignalR | WebSockets para el dashboard en tiempo real |
| Serilog | Logging estructurado a consola |
| QuestPDF | Generación de reportes PDF |
| ClosedXML | Generación de reportes Excel |
| FluentValidation | Validación de DTOs (donde aplica) |

---

## 📐 Módulos (Controllers)

| Controller | Endpoints clave | Lógica destacada |
|---|---|---|
| `AuthController` | `login`, `register`, `seed-admin` | `seed-admin` solo funciona si no existe ningún usuario — evita puerta trasera post-setup |
| `ObrasController` | CRUD + `PATCH /estado` | Valida transiciones de estado en el backend, no confía en el frontend |
| `MaterialesController` | CRUD | El campo `stock` nunca se edita directo — solo cambia vía Compras/Consumo |
| `ProveedoresController` | CRUD | — |
| `ComprasController` | `POST`, `POST /recepcion` | Recepción dentro de una transacción: incrementa stock de todos los materiales de la orden o revierte todo |
| `ConsumoController` | `POST` | Transacción que valida stock disponible antes de descontar — rechaza si no alcanza |
| `EmpleadosController` / `AsistenciasController` | CRUD + `entrada`/`salida` | Una asistencia por empleado/obra/día; rechaza doble entrada con 409 |
| `DashboardController` | `GET /{obraId}` | Agrega presupuesto, gasto, avance y materiales críticos de una obra en una sola respuesta |
| `NotificacionesController` | `GET`, `PATCH /leida` | Consulta de alertas generadas por el motor de automatización |
| `ReportesController` | `GET /excel`, `GET /pdf` | Genera los archivos en memoria y los devuelve como descarga directa |
| `FacturasController` / `FotosObraController` | `POST` (multipart) | Sube archivos a disco con nombre único (GUID), guarda la ruta relativa en BD |
| `LogsController` | `GET` (solo Admin) | Consulta de auditoría |

---

## 🤖 Motor de automatización — `AutomationEngine`

Un `BackgroundService` nativo de .NET que corre en un loop infinito, independiente del ciclo de request/response HTTP:

```csharp
public class AutomationEngine : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await EjecutarCicloAsync(stoppingToken);
            await Task.Delay(Intervalo, stoppingToken); // 15 minutos
        }
    }
}
```

Cada ciclo:
1. **Revisa stock bajo**: compara `Stock` vs `StockMinimo` de cada material activo.
2. **Revisa presupuesto**: suma el gasto (compras recibidas) por obra activa/pausada y lo compara contra el presupuesto asignado; alerta al superar el 90%.
3. **Genera notificaciones** de tipo `StockBajo`, `PresupuestoExcedido` y `CompraSugerida` — con lógica anti-duplicados: no crea una nueva alerta si ya existe una sin leer del mismo tipo para la misma obra/material.

Detalles de diseño relevantes:
- Usa `IServiceScopeFactory` para crear su propio scope de dependencias en cada ciclo, porque un `BackgroundService` vive más tiempo que cualquier request HTTP y no puede compartir el `DbContext` scoped normal.
- Un error dentro de un ciclo se loguea pero **no tumba el servicio** — el siguiente ciclo se ejecuta igual.
- Verificado en vivo bajando el intervalo a 10 segundos temporalmente durante desarrollo, confirmando detección, generación de alertas, y ausencia de duplicados en ciclos sucesivos.

---

## ⚡ Tiempo real — SignalR

### `DashboardHub`
Los clientes se unen a un grupo por obra (`obra-{id}`) al conectarse desde el frontend.

### `IRealtimeNotifier` — el patrón para no romper Clean Architecture
`CompraRepository` y `ConsumoRepository` (en `Infrastructure`) necesitan emitir eventos de SignalR tras una operación exitosa, pero el `Hub` vive en la capa `API`. Como `Infrastructure` no puede depender de `API` (violaría la regla de dependencias), se definió una interfaz `IRealtimeNotifier` en `Application`:

```
Application/Interfaces/IRealtimeNotifier.cs   → contrato ("qué" se notifica)
API/Realtime/SignalRRealtimeNotifier.cs       → implementación real con IHubContext<DashboardHub>
```

`Infrastructure` solo conoce la interfaz; la implementación concreta se inyecta desde `API` en el arranque. Así, `CompraRepository.RecepcionarAsync()` emite `"CompraRecepcionada"` y `ConsumoRepository.RegistrarAsync()` emite `"ConsumoRegistrado"`, ambos **después** de confirmar la transacción de base de datos, sin que `Infrastructure` sepa nada de SignalR.

### Autenticación del Hub
SignalR no puede mandar el JWT por header `Authorization` en el handshake WebSocket, así que se lee desde query string:

```csharp
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        var accessToken = context.Request.Query["access_token"];
        if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
            context.Token = accessToken;
        return Task.CompletedTask;
    }
};
```

---

## 🔐 Autenticación y seguridad

- **JWT + BCrypt**: contraseñas nunca se guardan en texto plano; los tokens incluyen claims de `sub`, `email`, `name` y `role`.
- **`MapInboundClaims = false`**: por defecto, .NET remapea claims cortos (`"sub"`) a URIs largas de `ClaimTypes` al validar el token entrante — esto rompía `User.FindFirst("sub")` en los controllers con un `NullReferenceException` silencioso. Desactivarlo hace que los claims lleguen exactamente como se generaron.
- **Autorización por rol** en cada endpoint sensible (`[Authorize(Roles = "Admin,JefeObra")]`), no solo autenticación genérica.
- **CORS configurable** vía variable de entorno `AllowedOrigins`, para permitir distintos orígenes en desarrollo (`localhost:4200`) y producción (dominio de Vercel) sin recompilar.

---

## 💾 Transacciones críticas

Dos operaciones usan transacciones explícitas de EF Core (`BeginTransactionAsync`) porque modifican múltiples entidades relacionadas y deben ser atómicas:

**Recepción de compra** (`CompraRepository.RecepcionarAsync`): incrementa el stock de cada material de la orden y marca la compra como `Recibida`. Si algo falla a mitad de camino, se hace rollback completo — nunca queda el stock actualizado sin marcar la compra como recibida, ni viceversa.

**Registro de consumo** (`ConsumoRepository.RegistrarAsync`): valida que haya stock suficiente, lo descuenta, y crea el registro de consumo — todo o nada. Si no hay stock suficiente, la operación se rechaza sin tocar la base de datos.

---

## 🚀 Cómo correr el backend localmente

### Requisitos
- .NET 10 SDK
- PostgreSQL local, o una base en [Neon](https://neon.tech) (gratis, sin tarjeta)

### Pasos

```bash
cd ConstruControl.API
dotnet restore
```

Crea `appsettings.Development.json` (no se sube al repo):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=construcontrol;Username=postgres;Password=tu_password"
  },
  "Jwt": {
    "Key": "una_clave_secreta_de_al_menos_32_caracteres_aqui"
  }
}
```

Aplica las migraciones y levanta el servidor:

```bash
dotnet ef database update --project ../ConstruControl.Infrastructure --startup-project .
dotnet run
```

Disponible en `http://localhost:5158`.

### Crear el primer usuario Admin

```bash
curl -X POST http://localhost:5158/api/auth/seed-admin \
  -H "Content-Type: application/json" \
  --data-binary '{"nombreCompleto":"Tu Nombre","email":"admin@construcontrol.com","password":"Admin123!","rol":"Admin"}'
```

---

## 🐳 Despliegue con Docker

El `Dockerfile` en la raíz de `backend/` compila en dos etapas (build con SDK, runtime con `aspnet` más liviano):

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# ... restore y publish ...

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*
# ... copiar publish y ENTRYPOINT ...
```

**`libgssapi-krb5-2`** no es opcional: Npgsql (el driver de PostgreSQL) la necesita para negociar la conexión con Neon, y la imagen `aspnet` (a diferencia de `sdk`) no la incluye por defecto — sin esto, el contenedor arranca pero falla en el primer intento de conexión a la base de datos.

### Variables de entorno en producción (Render)

```env
ConnectionStrings__DefaultConnection=Host=...neon.tech;Database=neondb;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true
Jwt__Key=clave_secreta_larga
Jwt__Issuer=ConstruControlAPI
Jwt__Audience=ConstruControlClient
AllowedOrigins=https://tu-dominio.vercel.app
```

> El doble guion bajo (`__`) es la convención de ASP.NET Core para representar la jerarquía anidada de `appsettings.json` en variables de entorno planas (`ConnectionStrings:DefaultConnection` → `ConnectionStrings__DefaultConnection`).

### Endpoint de salud

```
GET /health → { "status": "ok" }
```

Sin autenticación, pensado para monitoreo externo (UptimeRobot) que mantiene el servicio despierto en el plan gratuito de Render.

---

## 📦 Modelo de datos

13 tablas principales: `Usuarios`, `Obras`, `Materiales`, `Proveedores`, `Compras`, `DetalleCompra`, `Consumos`, `Empleados`, `Asistencias`, `Facturas`, `FotosObra`, `Notificaciones`, `Logs`. Ver [`docs/modelo-datos.md`](../docs/modelo-datos.md) y [`docs/schema.sql`](../docs/schema.sql) para el detalle completo de columnas y relaciones (el script SQL se mantiene como referencia; el esquema real en producción lo gobiernan las migraciones de EF Core).

---

## 👨‍💻 Autor

**Ronny Villa** — [GitHub](https://github.com/Ronnyvilla3021)
Parte del proyecto [ConstruControl](../README.md).
