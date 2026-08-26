# Arquitectura — ConstruControl

## Backend (.NET)
- Clean Architecture en 4 capas: Domain, Application, Infrastructure, API
- ORM: Entity Framework Core (Code First + Migrations)
- Auth: JWT + Refresh Tokens, roles (Admin, JefeObra, Bodeguero, Empleado)
- Acceso a datos: Repository + Unit of Work
- Validación: FluentValidation
- Logging: Serilog
- Tiempo real: SignalR
- Automatización: BackgroundService (motor de reglas: stock bajo, presupuesto, compra sugerida)
- Reportes: QuestPDF (PDF), ClosedXML (Excel)

## Frontend (Angular)
- Standalone components (sin NgModules)
- State: Signals (toSignal / computed)
- HTTP: interceptors para JWT y manejo global de errores
- UI Kit: PrimeNG
- Tiempo real: @microsoft/signalr

## Base de datos
- SQL Server
- Nombre: ConstruControlDB
- Convención: entidades en singular en C#, tablas en plural en SQL

## Decisiones registradas
- (aquí vamos anotando decisiones importantes a medida que avanzamos, con fecha)
