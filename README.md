# Management Platform

Plataforma de gestión de proyectos y tareas — Technical Assessment.

Arquitectura: **Clean Architecture** con .NET 8, PostgreSQL, Entity Framework Core y JWT.

---

## Estructura del proyecto

```
Management_Platform.sln
├── Platform.Domain          # Entidades, interfaces, excepciones de negocio
├── Platform.Application     # DTOs, servicios de aplicación, interfaces
├── Platform.Infrastructure  # EF Core, repositorios, AuthService, migraciones
├── Platform.Api             # API REST (ASP.NET Core + Swagger + JWT)
├── Platform.Web             # Interfaz web (ASP.NET Core MVC + Razor)
└── Platform.Tests           # Pruebas unitarias (xUnit)
```

---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 14+](https://www.postgresql.org/)
- (Opcional) [pgAdmin](https://www.pgadmin.org/) para visualizar la base de datos

---

## Configuración de la base de datos

1. Crea una base de datos en PostgreSQL:
   ```sql
   CREATE DATABASE management_platform;
   ```

2. Actualiza la cadena de conexión en los archivos `appsettings.json` de `Platform.Api` y `Platform.Web`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=management_platform;Username=TU_USUARIO;Password=TU_PASSWORD"
   }
   ```

---

## Migraciones

Ejecuta el siguiente comando desde la raíz del proyecto para aplicar las migraciones:

```bash
dotnet ef database update --project Platform.Infrastructure --startup-project Platform.Api
```

Para crear una nueva migración:

```bash
dotnet ef migrations add NombreMigracion --project Platform.Infrastructure --startup-project Platform.Api
```

---

## Cómo ejecutar

### API REST

```bash
dotnet run --project Platform.Api
```

Accede a Swagger en: `https://localhost:{puerto}/swagger`

### Web MVC

```bash
dotnet run --project Platform.Web
```

Accede en: `https://localhost:{puerto}`

---

## Autenticación (JWT)

La API usa JWT. Para obtener un token:

1. **Registrar un usuario** — `POST /api/auth/register`
   ```json
   { "email": "admin@test.com", "password": "Admin1234!" }
   ```

2. **Iniciar sesión** — `POST /api/auth/login`
   ```json
   { "email": "admin@test.com", "password": "Admin1234!" }
   ```
   La respuesta incluye el token JWT.

3. En Swagger, haz clic en **"Authorize"** e ingresa: `Bearer {tu_token}`

### Credenciales de prueba

| Email | Password |
|---|---|
| `admin@test.com` | `Admin1234!` |

> Debes registrar este usuario tú mismo la primera vez usando el endpoint `/api/auth/register`.

---

## Ejecutar pruebas unitarias

```bash
dotnet test Platform.Tests
```

**Tests incluidos:**
- `ActivateProject_WithTasks_ShouldSucceed`
- `ActivateProject_WithoutTasks_ShouldFail`
- `CompleteProject_WithAllTasksCompleted_ShouldSucceed`
- `CompleteProject_WithPendingTasks_ShouldFail`
- `CreateTask_WithInvalidOrder_ShouldFail`
- `DeleteProject_ShouldBeDeletedWhenEntityRemoved`

---

## Endpoints principales de la API

| Método | Ruta | Descripción | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Registrar usuario | ❌ |
| POST | `/api/auth/login` | Iniciar sesión | ❌ |
| GET | `/api/projects/search` | Listar proyectos (paginado) | ✅ |
| GET | `/api/projects/summary` | Resumen global | ✅ |
| POST | `/api/projects` | Crear proyecto | ✅ |
| PUT | `/api/projects/{id}` | Actualizar proyecto | ✅ |
| DELETE | `/api/projects/{id}` | Eliminar proyecto | ✅ |
| PATCH | `/api/projects/{id}/activate` | Activar proyecto | ✅ |
| PATCH | `/api/projects/{id}/complete` | Completar proyecto | ✅ |
| GET | `/api/projects/{id}/tasks` | Listar tareas del proyecto | ✅ |
| POST | `/api/tasks/{projectId}` | Crear tarea | ✅ |
| PUT | `/api/tasks/{id}` | Actualizar tarea | ✅ |
| DELETE | `/api/tasks/{id}` | Eliminar tarea | ✅ |
| PATCH | `/api/tasks/{id}/complete` | Completar tarea | ✅ |
| PATCH | `/api/tasks/{id}/reorder` | Reordenar tarea | ✅ |

---

## Tecnologías utilizadas

- **Backend**: ASP.NET Core 8
- **ORM**: Entity Framework Core 8
- **Base de datos**: PostgreSQL (via Npgsql)
- **Autenticación**: JWT (System.IdentityModel.Tokens.Jwt)
- **Hashing**: BCrypt.Net-Next
- **Documentación API**: Swagger / Swashbuckle
- **Pruebas**: xUnit + Moq
