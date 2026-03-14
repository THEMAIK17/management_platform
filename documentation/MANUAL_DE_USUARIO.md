# Manual de Usuario - Management Platform

## 1. Requisitos del Sistema
*   Tener instalado **.NET 8 SDK**.
*   Tener **PostreSQL** instalado y configurado.
*   Navegador web moderno (Chrome, Firefox, Edge).

## 2. Pasos de Instalación y Ejecución
1.  **Base de Datos**: Crear la base de datos `management_platform`.
2.  **Secretos**: Configurar la conexión en la terminal:
    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=management_platform;Username=postgres;Password=TU_PASSWORD" --project Platform.Web
    ```
3.  **Ejecutar**: Desde la raíz, ejecutar `dotnet run --project Platform.Web`.
4.  **Acceso**: Abrir `https://localhost:7154` (o el puerto indicado en consola).

## 3. Descripción de Funcionalidades
### Gestión de Proyectos
*   **Creación**: Permite registrar un proyecto con nombre y descripción. Los proyectos inician en estado "Draft" (Borrador).
*   **Activación**: Un proyecto solo puede pasar a "Activo" si tiene al menos una tarea asignada.
*   **Cierre**: Un proyecto solo se puede marcar como "Completado" si todas sus tareas están finalizadas.

### Gestión de Tareas
*   **Registro**: Asignar tareas a proyectos específicos con niveles de prioridad (Baja, Media, Alta).
*   **Progreso**: Marcar tareas como completadas.
*   **Reordenamiento**: Cambiar el orden de prioridad de las tareas.

## 4. Capturas de Pantalla del Sistema
*(A continuación se presentan los espacios para insertar las capturas de pantalla una vez el sistema esté en ejecución)*

### 4.1 Pantalla de Login
> ![Pantalla de Login](screenshots/login.png) - Introducción de credenciales seguras.

### 4.2 Tablero de Proyectos
> ![Tablero de Proyectos](screenshots/dashboard.png) - Listado general con estados (Draft, Active, Completed).

### 4.3 Gestión de Tareas
> ![Gestión de Tareas](screenshots/task-list.png) - Formulario de creación y listado de tareas con orden jerárquico.

---
*Nota: Para generar los reportes de usuario, el sistema utiliza vistas Razor dinámicas.*
