# RestauranteApi

API ASP.NET Core para administrar clientes, zonas, secciones, mesas, turnos,
reservas, bloqueos de mesas y lista de espera.

## Ejecutar localmente

La configuración de desarrollo incluida permite iniciar el proyecto:

```powershell
dotnet run
```

La API queda disponible por defecto en `http://localhost:5065`.

Para otros ambientes, configura como mínimo:

```text
Jwt__Key
AdminCredentials__Username
AdminCredentials__Password
```

No publiques credenciales reales dentro de `appsettings.json`.

## Endpoints principales

- OpenAPI: `GET /openapi/v1.json`
- Estado de la API: `GET /health`
- Login administrativo: `POST /api/auth/login`
- Catálogo público: `GET /api/zones`, `/api/sections`, `/api/tables`, `/api/turns`
- Mesas disponibles: `GET /api/tables/available?date=2026-06-20&time=13:00:00&capacity=2`
- Crear cliente: `POST /api/clients`
- Crear reserva: `POST /api/reservations`
- Lista de espera: `POST /api/waiting-list`
- Administración de bloqueos: `/api/table-locks`

Los endpoints administrativos requieren el encabezado:

```text
Authorization: Bearer <token>
```

Los listados paginados aceptan `page` y `pageSize`. `pageSize` debe estar entre
`1` y `100`.

## Base de datos

En desarrollo se crea una base SQLite local `restaurante.db` con datos iniciales.
El archivo está excluido de Git.
