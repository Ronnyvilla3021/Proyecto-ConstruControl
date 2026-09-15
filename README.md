## 🌐 Demo en vivo

- **Panel Web**: https://construcontrol-xi.vercel.app
- **API Backend**: https://proyecto-construcontrol.onrender.com

## 🔑 Credenciales de prueba

- **Email**: `admin@construcontrol.com`
- **Contraseña**: `Admin123!`

> Nota: el backend usa el plan gratuito de Render, así que la primera petición después de inactividad puede tardar unos 30-50 segundos en "despertar" el servidor.

# 🏗️ ConstruControl

Sistema integral de gestión de operaciones para empresas constructoras: control de materiales, compras, presupuesto y personal, con automatización en segundo plano y dashboard en tiempo real.


## 🎯 Sobre el proyecto

**ConstruControl** resuelve un problema real de las constructoras pequeñas y medianas: el descontrol de materiales y costos que aparece cuando el seguimiento se lleva en hojas de cálculo o cuadernos físicos. El sistema centraliza:

- El ciclo de vida completo de cada **obra**, desde planificación hasta finalización.
- El **inventario de materiales**, con umbral de stock mínimo configurable.
- Las **órdenes de compra** y su recepción, que actualiza el stock automáticamente dentro de una transacción atómica.
- El **consumo de materiales por obra**, validado contra el stock real disponible — nunca se puede consumir más de lo que existe físicamente.
- El **personal de campo**, con control de asistencia por entrada y salida.

Todo el gasto queda auditado: cada compra recibida y cada consumo quedan ligados a una obra, un material y un usuario responsable, con fecha y hora exactas.

---

## ✨ Funcionalidades destacadas

- 🔐 **Autenticación JWT con roles** (Admin, JefeObra, Bodeguero, Empleado) y autorización por endpoint según el rol real de cada acción.
- ⚡ **Dashboard en tiempo real vía SignalR (WebSockets)** — los eventos de compra recepcionada y consumo registrado aparecen en pantalla sin recargar, en cualquier cliente conectado a esa obra.
- 🔄 **Transacciones atómicas de stock**: recepcionar una compra incrementa el stock; registrar un consumo lo decrementa y rechaza la operación completa si no hay suficiente material — nunca queda la base de datos a medias.
- 🏗️ **Máquina de estados en Obras**: `Planificación → Activa → Pausada/Finalizada`, con validación de transiciones permitidas en el backend, no solo en la interfaz.
- 🤖 **Motor de automatización (BackgroundService)**: proceso que corre en segundo plano cada 15 minutos, detectando stock bajo y presupuesto cerca de agotarse, generando alertas y sugerencias de compra sin que nadie lo dispare manualmente.
- 📄 **Exportación de reportes** en Excel (compras/consumos por obra) y PDF (indicadores financieros), generados en el servidor con QuestPDF y ClosedXML.
- 📎 **Adjuntos**: subida de facturas ligadas a compras y fotos de avance ligadas a obras.
- 📝 **Auditoría**: registro de acciones sensibles (eliminaciones) con usuario, entidad y fecha.
- 🌗 **Modo claro/oscuro** persistente, con diseño responsive para escritorio, tablet y móvil.

---

## 🏗️ Arquitectura

```
Proyecto ConstruControl/
├── backend/
│   ├── ConstruControl.Domain/          → Entidades del negocio, sin dependencias externas
│   ├── ConstruControl.Application/     → Interfaces, DTOs, reglas de negocio
│   ├── ConstruControl.Infrastructure/  → EF Core, repositorios, servicios (JWT, storage, reportes)
│   └── ConstruControl.API/             → Controllers, SignalR, BackgroundService, configuración
├── frontend/                           → Angular 22 + PrimeNG
└── docs/                               → Modelo de datos, esquema SQL, decisiones de arquitectura
```

**Flujo general:** el frontend Angular consume la API REST vía HTTP con interceptor JWT automático, y se conecta al hub de SignalR para recibir eventos en tiempo real de la obra que se está visualizando. El backend sigue Clean Architecture: las capas internas (Domain, Application) no conocen nada de las externas (Infrastructure, API), lo que permitió migrar de SQL Server a PostgreSQL sin tocar una sola línea de lógica de negocio.

---

## 🛠️ Stack técnico

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core / .NET 10, Clean Architecture (4 capas) |
| Base de datos | PostgreSQL (Neon) — desarrollado originalmente en SQL Server |
| ORM | Entity Framework Core (Code First + Migrations) |
| Autenticación | JWT + BCrypt, roles por endpoint |
| Tiempo real | SignalR (WebSockets) |
| Automatización | `BackgroundService` nativo de .NET |
| Reportes | QuestPDF (PDF), ClosedXML (Excel) |
| Frontend | Angular 22 (standalone components, Signals) |
| UI Kit | PrimeNG, PrimeIcons |
| Logging | Serilog |

---

## 📐 Módulos del backend

| Módulo | Responsabilidad |
|---|---|
| Auth | Login JWT, registro con rol, hash de contraseñas con BCrypt |
| Obras | CRUD, transición de estados validada, presupuesto |
| Materiales | CRUD, stock, umbral mínimo de alerta |
| Proveedores | CRUD simple |
| Compras | Orden con múltiples materiales, recepción transaccional |
| Consumo | Registro validado contra stock disponible |
| Personal | Empleados y asistencia (entrada/salida) por obra |
| Dashboard | Agregación de presupuesto/gasto/avance + hub de SignalR |
| Automatización | `BackgroundService` de detección de stock bajo y presupuesto excedido |
| Reportes | Exportación Excel y PDF |
| Facturas / Fotos | Subida de archivos ligados a Compras y Obras |
| Logs | Auditoría de acciones sensibles |

**Documentación específica:**
- [Frontend README](./frontend/README.md) — arquitectura Angular, componentes, PrimeNG, SignalR
- [Backend README](./backend/README.md) — Clean Architecture, EF Core, controllers, automatización

---

## 🚀 Cómo correr el proyecto localmente

### Requisitos previos
- .NET 10 SDK
- Node.js 18+ y Angular CLI
- PostgreSQL (o SQL Server, con el paquete NuGet correspondiente) corriendo localmente

### 1. Backend

```bash
cd backend
dotnet restore

# Configura tu cadena de conexión en ConstruControl.API/appsettings.Development.json
# (ver ejemplo en la sección de variables de entorno más abajo)

dotnet ef database update --project ConstruControl.Infrastructure --startup-project ConstruControl.API
dotnet run --project ConstruControl.API
```

El servidor queda disponible en `http://localhost:5158`.

### 2. Frontend

```bash
cd frontend
npm install

# Verifica que src/environments/environment.ts apunte a tu backend local
ng serve
```

Disponible en `http://localhost:4200`.

---

## 🔑 Variables de entorno (backend)

En `ConstruControl.API/appsettings.Development.json` (excluido del repositorio):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=construcontrol;Username=postgres;Password=tu_password"
  },
  "Jwt": {
    "Key": "una_clave_secreta_de_al_menos_32_caracteres"
  }
}
```

En producción (Render), estas se configuran como variables de entorno del servicio:

```env
ConnectionStrings__DefaultConnection=Host=...;Database=...;Username=...;Password=...;SSL Mode=Require
Jwt__Key=...
Jwt__Issuer=ConstruControlAPI
Jwt__Audience=ConstruControlClient
AllowedOrigins=https://tu-dominio.vercel.app
```

---

## 👤 Roles del sistema

| Rol | Permisos |
|---|---|
| **Admin** | Acceso total: usuarios, eliminar registros, logs de auditoría |
| **JefeObra** | Crear/editar obras, cambiar estados, gestionar personal |
| **Bodeguero** | Materiales, proveedores, compras, recepción |
| **Empleado** | Registrar consumo, ver información general |

---

## 🐛 Problemas reales que aparecieron y cómo se resolvieron

Documentar esto tiene valor: cualquier proyecto real se topa con esto, y resolverlo (no evitarlo) es la parte que demuestra criterio técnico.

**1. Claims de JWT renombrados automáticamente por .NET** — el claim `"sub"` se remapeaba a una URI larga de `ClaimTypes`, causando `NullReferenceException` en los controllers. Se resolvió con `options.MapInboundClaims = false;`.

**2. SQL Server no tiene opción gratuita sin tarjeta para producción** — el proyecto se desarrolló en SQL Server, pero ningún hosting gratuito real lo ofrecía sin pedir tarjeta. Se migró a PostgreSQL (Neon): cambio de paquete NuGet, nueva migración de EF Core, y ninguna modificación en entidades ni lógica de negocio gracias a la abstracción de EF Core.

**3. Librería nativa faltante en Docker** — `Cannot load library libgssapi_krb5.so.2`, requerida por Npgsql para conectar con Neon. Se agregó `apt-get install libgssapi-krb5-2` en la imagen final del Dockerfile.

**4. Frontend en producción apuntando a `localhost`** — faltaba el bloque `fileReplacements` en la configuración `production` de `angular.json`, por lo que el build usaba siempre el archivo de entorno de desarrollo.

**5. CORS bloqueaba Vercel** — el origen permitido se hizo configurable vía variable de entorno `AllowedOrigins`, en vez de estar fijo en el código.

**6. Render duerme el servicio gratuito** — se configuró un endpoint `/health` público monitoreado por UptimeRobot cada 5 minutos.

---

## 📄 Licencia

MIT — libre de usar como referencia o base para tus propios proyectos.

---

## 👨‍💻 Autor

**Ronny Villa** — [GitHub](https://github.com/Ronnyvilla3021)
Proyecto desarrollado como parte de un portafolio profesional full-stack — parte de una serie de 3 proyectos: **Fiado Digital** (finanzas/créditos), **Bodega Control Pro** (inventario/móvil), **ConstruControl** (operaciones/empresa).
