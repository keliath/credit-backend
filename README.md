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

## Instalación y ejecución local

### 1. Clona el repositorio

### 2. Configura la base de datos
- Crea una base de datos SQL Server (puedes usar SQL Server Express o una instancia local/remota).
- Actualiza la cadena de conexión en `src/CreditApp.API/appsettings.json`:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=creditapp;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
  }
  ```

### 3. Restaura dependencias y aplica migraciones
```bash
dotnet restore
cd src/CreditApp.API
dotnet ef database update
```

### 4. Ejecuta la aplicación
```bash
dotnet run --project src/CreditApp.API
```
La API estará disponible en `http://localhost:5219` o el puerto configurado.

---


## Endpoints principales
- `POST /api/auth/login` — Login de usuario
- `POST /api/auth/register` — Registro de usuario
- `GET /api/auth/whoami` — Información del usuario autenticado
- `GET /api/creditrequest` — Listado paginado de solicitudes (requiere rol Analista)
- `POST /api/creditrequest` — Crear solicitud de crédito
- `PUT /api/creditrequest/{id}/status` — Cambiar estado (requiere rol Analista)
- `DELETE /api/creditrequest/{id}` — Eliminar solicitud

---
