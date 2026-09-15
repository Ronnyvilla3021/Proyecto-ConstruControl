# 🎨 ConstruControl — Frontend

Panel web de administración construido en **Angular 22**, con componentes standalone, Signals para manejo de estado reactivo, y PrimeNG como sistema de diseño. Consume la API REST del backend y se conecta al hub de SignalR para actualizaciones en tiempo real.

## 🛠️ Stack técnico

| Herramienta | Uso |
|---|---|
| Angular 22 | Framework base — standalone components, sin NgModules |
| Signals | Estado reactivo (`signal`, `computed`, `effect`) en lugar de solo RxJS/Observables para estado local |
| PrimeNG 22 | Librería de componentes UI (tablas, diálogos, formularios, cards) |
| PrimeIcons | Iconografía |
| Reactive Forms | Formularios con validación (`FormBuilder`, `FormGroup`, `FormArray`) |
| `@microsoft/signalr` | Cliente de WebSockets para el Dashboard en tiempo real |
| HttpClient + Interceptors | Comunicación con la API, inyección automática de JWT |

---

## 🏗️ Arquitectura de carpetas

```
frontend/src/app/
├── core/
│   ├── guards/          → authGuard (protege rutas que requieren sesión)
│   ├── interceptors/     → authInterceptor (agrega el JWT a cada request)
│   ├── models/           → Interfaces TypeScript que reflejan los DTOs del backend
│   └── services/         → Un servicio HTTP por entidad + AuthService + ThemeService + SignalrService
├── features/
│   ├── auth/login/               → Pantalla de login (fija, no cambia con el tema)
│   ├── obras/obras-list/         → CRUD de obras + cambio de estado
│   ├── materiales/materiales-list/
│   ├── compras/compras-list/     → Orden con múltiples materiales (FormArray) + recepción
│   ├── consumo/consumo-list/     → Registro de consumo con validación de stock
│   ├── personal/personal-list/   → Empleados + asistencias (entrada/salida)
│   └── dashboard/
│       ├── resumen-general/      → Pantalla de inicio: stat cards + todas las obras + alertas
│       └── dashboard/            → Detalle de una obra específica, conectado a SignalR
├── app.ts / app.html / app.scss  → Layout principal: sidebar + topbar + toggle de tema
├── app.routes.ts                 → Rutas con lazy loading
└── app.config.ts                 → Configuración de PrimeNG (tema, modo oscuro), HttpClient
```

**Patrón por módulo:** cada feature bajo `features/` sigue la misma estructura — un servicio en `core/services` que habla con la API, un componente standalone con signals para el estado local, y un `.html`/`.scss` propio. No hay estado global compartido (NgRx, etc.) porque la complejidad del dominio no lo justificaba; cada componente carga sus propios datos al inicializarse.

---

## ⚡ Signals en lugar de solo Observables

Angular 22 permite manejar estado local con `signal()` en vez de depender de `BehaviorSubject`/`async pipe` para todo. En este proyecto:

```typescript
obras = signal<Obra[]>([]);
cargando = signal(false);
totalObrasActivas = computed(() => this.obras().filter(o => o.estado === 'Activa').length);
```

- Los datos que vienen de la API (`obtenerTodas()`) siguen siendo `Observable` (así es como funciona `HttpClient`), pero al recibir la respuesta se guardan en un `signal`.
- Los valores derivados (totales, filtros, contadores) usan `computed()`, que solo se recalcula cuando su dependencia cambia — no hay que gestionar suscripciones manualmente ni preocuparse por memory leaks de observables no cerrados.
- El HTML usa la sintaxis de control de flujo nativa de Angular (`@if`, `@for`) en vez de `*ngIf`/`*ngFor`.

---

## 🔐 Autenticación y seguridad

### `AuthService`
Maneja el login, guarda el token en `localStorage`, y expone el estado de sesión como signals (`estaAutenticado`, `usuario`, `rol`) que el resto de la app puede leer reactivamente.

Un detalle importante: el servicio **valida la expiración del token JWT en el cliente**, decodificando el payload (`exp`) sin necesidad de una librería externa:

```typescript
private tokenExpirado(token: string): boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return Date.now() >= payload.exp * 1000;
  } catch {
    return true;
  }
}
```

Esto evita el escenario de "sesión fantasma": sin esta validación, un usuario con un token vencido de días atrás seguía viéndose "logueado" en la interfaz aunque cada petición al backend fallara con 401.

### `authInterceptor`
Interceptor funcional (`HttpInterceptorFn`) que agrega el header `Authorization: Bearer <token>` a cada request saliente, sin que cada servicio tenga que hacerlo manualmente.

### `authGuard`
Protege las rutas: si no hay sesión válida, redirige a `/login` antes de que el componente se cargue.

---

## ⚡ Tiempo real con SignalR

`SignalrService` gestiona la conexión al hub `/hubs/dashboard` del backend. El token JWT se pasa por **query string** (no por header), porque el protocolo WebSocket no permite headers HTTP personalizados en el handshake inicial:

```typescript
.withUrl(`${environment.hubUrl}?access_token=${token}`)
```

El componente `Dashboard` (detalle de una obra) se suscribe a los eventos `CompraRecepcionada` y `ConsumoRegistrado`, y actualiza la vista automáticamente — sin polling, sin recargar la página — usando un `effect()` que reacciona al signal `ultimoEvento`.

---

## 🎨 Diseño y sistema de temas

- **PrimeNG con preset personalizado** (`definePreset` sobre el tema Aura): color primario ámbar, paleta de superficies oscuras propia para que tablas, diálogos y cards se vean coherentes en modo oscuro (no solo el layout general).
- **Modo oscuro/claro con persistencia**: `ThemeService` aplica la clase `.app-dark` al `<html>` según la preferencia guardada en `localStorage`. El modo oscuro **solo se activa si hay sesión iniciada** — el login siempre se ve igual, sin importar la preferencia de tema del usuario.
- **Responsive real**: el sidebar colapsa a un menú hamburguesa por debajo de 960px de ancho, con overlay para cerrarlo al tocar fuera — probado en tamaños de tablet y móvil, no solo desktop.

---

## 🚀 Cómo correr el frontend localmente

### Requisitos
- Node.js 18+
- Angular CLI (`npm install -g @angular/cli`, opcional — también funciona con `npx`)

### Pasos

```bash
npm install

# Verifica que src/environments/environment.ts apunte a tu backend local:
# apiUrl: 'http://localhost:5158/api'
# hubUrl: 'http://localhost:5158/hubs/dashboard'

ng serve
```

Disponible en `http://localhost:4200`. Requiere que el [backend](../backend/README.md) esté corriendo.

### Build de producción

```bash
ng build --configuration production
```

El output queda en `dist/construcontrol-frontend/browser/`. La configuración `production` en `angular.json` incluye un `fileReplacements` que sustituye `environment.ts` por `environment.prod.ts` automáticamente — **importante**: sin ese bloque, el build de producción sigue apuntando a `localhost` aunque compile sin errores (fue un bug real detectado ya en producción, documentado en el README raíz).

---

## 📦 Dependencias clave

```json
"@angular/core": "^22.1.0",
"@microsoft/signalr": "^8.0.0",
"primeng": "^22.1.1",
"@primeuix/themes": "^1.x",
"primeicons": "^7.x"
```

> Nota: `@primeng/themes` está deprecado a favor de `@primeuix/themes` — este proyecto usa el paquete correcto y mantenido.

---

## 🌐 Entornos

| Archivo | Uso |
|---|---|
| `environment.ts` | Desarrollo local — apunta a `http://localhost:5158` |
| `environment.prod.ts` | Producción — apunta al backend desplegado en Render |

---

## 👨‍💻 Autor

**Ronny Villa** — [GitHub](https://github.com/Ronnyvilla3021)
Parte del proyecto [ConstruControl](../README.md).
