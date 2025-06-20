# CreditApp Backend

Backend para la gestión de solicitudes de crédito con autenticación JWT, roles, auditoría automática y arquitectura limpia basada en MediatR.

## Características principales

- API RESTful para gestión de usuarios y solicitudes de crédito
- Autenticación y autorización con JWT
- Roles: Solicitante y Analista
- Auditoría automática de todas las operaciones críticas (login, registro, CRUD de solicitudes)
- Paginación en endpoints de listados
- Arquitectura limpia con MediatR para comandos y queries

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local o remoto)

---

## Puesta en Marcha con Docker

#### Requisitos Previos

- Docker engine o desktop
- Make (Opcional), en linux y mac debería venir por defecto en windows: con el manejador de paquetes chocolatey `choco install make`.

  1.1. **Ejecutar la aplicación con Makefile**

```bash
make dev
```

1.2 **Ejecutar la aplicación sin Makefile**

```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml
```
La API estará disponible en `http://localhost:5219` o el puerto configurado.

## Instalación y ejecución local

### 1. Configura la base de datos

- Crea una base de datos SQL Server (puedes usar SQL Server Express o una instancia local/remota).
- Actualiza la cadena de conexión en `src/CreditApp.API/appsettings.json`:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=creditapp;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
  }
  ```

### 2. Restaura dependencias y aplica migraciones

```bash
dotnet restore

- En caso de que se usen migraciones
dotnet ef migrations add InitialCreate --project src/CreditApp.Infrastructure --startup-project src/CreditApp.API

dotnet ef database update --project src/CreditApp.Infrastructure --startup-project src/CreditApp.API
```

### 3. Ejecuta la aplicación

```bash
dotnet run --project src/CreditApp.API
```

La API estará disponible en `http://localhost:5219` o el puerto configurado.

---

## Usuarios de prueba:

email: analyst1@example.com
password: Password123!

email: user1@example.com
password: Password123!

## Endpoints principales

- `POST /api/auth/login` — Login de usuario
- `POST /api/auth/register` — Registro de usuario
- `GET /api/auth/whoami` — Información del usuario autenticado
- `GET /api/creditrequest` — Listado paginado de solicitudes (requiere rol Analista)
- `POST /api/creditrequest` — Crear solicitud de crédito
- `PUT /api/creditrequest/{id}/status` — Cambiar estado (requiere rol Analista)
- `DELETE /api/creditrequest/{id}` — Eliminar solicitud
- `GET /api/creditrequest/export` — Exportar solicitudes a Excel (requiere rol Analista)
- `POST /api/creditrequest/export` — Exportar solicitudes a Excel con filtros (requiere rol Analista)

---
