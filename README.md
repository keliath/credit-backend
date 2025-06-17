# Sistema de Gestión de Créditos

## Descripción
Sistema de gestión de créditos desarrollado siguiendo los principios de Domain-Driven Design (DDD) y Clean Architecture. Permite a los usuarios solicitar créditos y a los analistas revisar y aprobar/rechazar las solicitudes.

## Características Principales
- Autenticación y autorización basada en roles (Usuario, Analista, Administrador)
- Gestión de solicitudes de crédito
- Validación de datos y reglas de negocio
- Auditoría de acciones
- API RESTful
- Base de datos SQL Server
- Arquitectura limpia y mantenible

## Tecnologías Utilizadas
- .NET 8
- Entity Framework Core
- SQL Server
- JWT para autenticación
- BCrypt para hash de contraseñas
- Swagger/OpenAPI para documentación

## Estructura del Proyecto
```
src/
├── CreditApp.API/              # Capa de presentación (API)
├── CreditApp.Application/      # Capa de aplicación (casos de uso)
├── CreditApp.Domain/          # Capa de dominio (entidades y reglas)
└── CreditApp.Infrastructure/  # Capa de infraestructura (persistencia)
```

## Requisitos Previos
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 o VS Code

## Configuración del Entorno

1. Clonar el repositorio:


2. Configurar la base de datos:
   - Crear una base de datos SQL Server llamada `CreditAppDb`
   - Actualizar la cadena de conexión en `appsettings.json`

3. Restaurar dependencias:
```bash
dotnet restore
```

4. Aplicar migraciones:
```bash
dotnet ef database update --project src/CreditApp.Infrastructure --startup-project src/CreditApp.API
```

5. Ejecutar la aplicación:
```bash
dotnet run --project src/CreditApp.API
```

## Endpoints de la API

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar nuevo usuario

### Solicitudes de Crédito
- `POST /api/creditrequest` - Crear solicitud
- `GET /api/creditrequest/my-requests` - Ver mis solicitudes
- `GET /api/creditrequest` - Ver todas las solicitudes (Analista)
- `PUT /api/creditrequest/{id}/status` - Actualizar estado (Analista)

## Usuarios por Defecto
```
Usuario Regular:
- Email: user1@example.com
- Password: Password123!

Analista:
- Email: analyst1@example.com
- Password: Password123!

Administrador:
- Email: admin1@example.com
- Password: Password123!
```

## Características de Seguridad
- Autenticación JWT
- Contraseñas hasheadas con BCrypt
- Validación de roles
- Protección contra inyección SQL
- Auditoría de acciones

## Manejo de Errores
- Respuestas HTTP apropiadas
- Mensajes de error descriptivos
- Logging de errores
- Validación de datos 