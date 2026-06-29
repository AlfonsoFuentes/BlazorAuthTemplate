# Contexto de colaboración - ProjectToolV-2

> Archivo de contexto para retomar la conversación con OpenCode.
> Fecha última sesión: 2026-06-29
> Proyecto: Aplicación de Project Management (Blazor WASM + ASP.NET Core Minimal APIs)

---

## 1. Reglas de colaboración obligatorias

### 1.1. Autorización previa para cualquier modificación
> **IMPORTANTE**: Antes de modificar cualquier archivo en el proyecto, debo **proponer los cambios primero** y esperar la aprobación explícita del usuario.
>
> **Flujo correcto**:
> 1. Propongo la modificación (explico qué haré y por qué).
> 2. El usuario revisa y responde.
> 3. **Solo si el usuario dice explícitamente que esté de acuerdo** (ej: "actualiza", "hazlo", "adelante", "ok"), ejecuto los cambios en el proyecto.
>
> **NO debo** editar archivos, crear archivos nuevos, ni ejecutar comandos que modifiquen el código sin autorización previa del usuario, salvo que me lo haya pedido explícitamente en ese mismo mensaje.

### 1.2. Cumplimiento estricto de principios de programación
> **Toda sugerencia, refactorización o nuevo código debe cumplir estrictamente**:
> - **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion.
> - **DRY** (Don't Repeat Yourself): Cero duplicación de lógica. Si algo se repite, se abstrae.
> - **KISS** (Keep It Simple, Stupid): La solución más simple es la mejor. Evitar sobre-ingeniería.
> - **YAGNI** (You Aren't Gonna Need It): No escribir código "por si acaso". Solo lo que se necesita ahora.
> - **Design Patterns**: Aplicar patrones de diseño cuando aportan claridad (Factory, Strategy, Facade, Template Method, etc.).
> - **Buenas prácticas generales**: Null-safety, manejo de excepciones, logging, thread-safety donde aplica, validaciones defensivas.

### 1.3. UI/UX — MudBlazor como framework principal
> **MudBlazor 9.x está instalado y es el framework UI principal**. Se usa extensivamente para:
> - Layouts (`MudLayout`, `MudAppBar`, `MudDrawer`, `MudMainContent`).
> - Formularios (`MudForm`, `MudTextField`, `MudSelect`, `MudButton`, `MudDialog`).
> - Tablas (`MudTable`, `MudDataGrid`).
> - Notificaciones (`ISnackbar`).
> - Grids responsivos (`MudGrid`, `MudItem`).
> - Tooltips, Tabs, Expansion Panels, Loading indicators.
>
> HTML manual + CSS scoped se usa **solo** cuando MudBlazor no cubre el caso (renderizado SVG custom, animaciones específicas).

---

## 2. Descripción completa de la arquitectura

### 1.1. Estructura general
El proyecto sigue una arquitectura **Client-Server** con tres capetas principales:

```
ProjectToolV-2/
├── Server/                 → ASP.NET Core (API + Hosting del WASM)
├── CllientMudBlazor/       → Blazor WebAssembly (UI con MudBlazor)
└── Shared/                 → DTOs, Enums y contratos compartidos
```

### 1.2. Servidor (ASP.NET Core)
El servidor usa **Minimal APIs** como patrón principal para exponer endpoints de negocio. **NO usa controllers** para las operaciones CRUD (salvo autenticación).

#### Capas principales
| Carpeta | Propósito |
|---------|-----------|
| `DataContext/` | `AppDbContext` (EF Core + Identity + Tenant + Cache) |
| `Domain/` | Entidades del dominio (`Project`, `BudgetItem`, `StakeHolder`, etc.) |
| `EndPoints/` | Clases que implementan `IEndPoint` y mapean Minimal APIs |
| `Interfaces/` | Contratos (`IEndPoint`, `IEntity`, `ISoftDeletable`, `ITennant`) |
| `Services/` | Registro de servicios (`RegisterServices`, `AppBuilder`, `TokenService`) |
| `Controllers/` | **Solo** autenticación (`AuthorizationController` para Login/Register) |
| `Migrations/` | Migraciones de EF Core |

#### Tecnologías clave
- **EF Core** + SQL Server (conexión en `appsettings.json`)
- **ASP.NET Core Identity** (`AppUser : IdentityUser`)
- **JWT Bearer** para autenticación
- **LazyCache** para caché en memoria del servidor
- **QuestPDF** para generación de reportes PDF

### 1.3. Cliente (Blazor WASM + MudBlazor)
El cliente es una Single Page Application (SPA) que corre completamente en el navegador.

#### Capas principales
| Carpeta | Propósito |
|---------|-----------|
| `Pages/` | Componentes `.razor` organizados por funcionalidad |
| `Services/` | Servicios de infraestructura (HTTP, Auth, Notificaciones, Snackbar) |
| `Templates/` | Componentes reutilizables genéricos |
| `Layout/` | Layouts principales (`MainLayout.razor`) |

#### Tecnologías clave
- **Blazor WebAssembly** (client-side)
- **MudBlazor** (componentes UI: tablas, diálogos, formularios, Snackbar)
- **Blazored.LocalStorage** (persistencia del JWT en `localStorage`)
- **Toolbelt.Blazor.HttpClientInterceptor** (intercepción global de HTTP requests/responses)
- **JWT manual** (parseo local del token para mostrar claims en UI)

---

## 3. Cómo se crean los endpoints

### 2.1. Patrón `IEndPoint`
Todos los endpoints de negocio implementan una interfaz única:

```csharp
public interface IEndPoint
{
    void MapEndPoint(IEndpointRouteBuilder app);
}
```

### 2.2. Convención de nomenclatura
El nombre del endpoint (en la URL) **es idéntico** al nombre de la clase DTO que recibe. Esto es un estándar del proyecto.

```csharp
// DTO
public class DeleteProject { public Guid Id { get; set; } }

// Endpoint
app.MapPost("DeleteProject", async (DeleteProject dto, IAppDbContext _context) => { ... });
```

### 2.3. Registro automático por reflexión
Los endpoints **no se registran manualmente** uno por uno. Usan reflexión:

En `Server/Services/RegisterServices.cs`:
```csharp
builder.Services.AddEndPoints(); // Busca automáticamente todas las clases que implementan IEndPoint
```

En `Server/Services/AppBuilder.cs`:
```csharp
var apiGroup = app.MapGroup("").RequireAuthorization(); // Todos los endpoints requieren JWT

using (var scope = app.Services.CreateScope())
{
    var endpoints = scope.ServiceProvider.GetServices<IEndPoint>();
    foreach (var endpoint in endpoints)
    {
        endpoint.MapEndPoint(apiGroup); // Cada clase mapea sus rutas
    }
}
```

### 2.4. Ejemplo completo de un endpoint
Tomemos `BrandEndPoints` como referencia del patrón:

```csharp
public class BrandEndPoints : IEndPoint
{
    public void MapEndPoint(IEndpointRouteBuilder app)
    {
        // CREATE
        app.MapPost("CreateBrand", async (CreateBrand dto, IAppDbContext _context) =>
        {
            var row = new Brand { Id = Guid.NewGuid() };
            MapFromDto(dto, row);
            await _context.Brands.AddAsync(row);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _context.InvalidateCache($"{typeof(GetAllBrands).Name}");
                return Results.Ok(new GeneralDto { Succeeded = true, Message = "..." });
            }
            return Results.Ok(new GeneralDto { Succeeded = false, Message = "..." });
        });

        // DELETE (Soft Delete)
        app.MapPost("DeleteBrand", async (DeleteBrand dto, IAppDbContext _context) =>
        {
            var row = await _context.Brands.FindAsync(dto.Id);
            if (row is null) return Results.Ok(new GeneralDto { Succeeded = false, ... });
            row.IsDeleted = true;
            if (await _context.SaveChangesAsync() > 0)
            {
                _context.InvalidateCache(cacheKeyId, cacheKeyAll);
                return Results.Ok(new GeneralDto { Succeeded = true, ... });
            }
            return Results.Ok(new GeneralDto { Succeeded = false, ... });
        });
    }
}
```

### 2.5. Patrones de respuesta
Todos los endpoints devuelven `Results.Ok(new GeneralDto { ... })` o `Results.Ok(new GeneralDto<T> { ... })`, **incluso en errores**. Nunca devuelven códigos HTTP 400/500 crudos (salvo 401 del middleware JWT).

La clase base de respuesta está en `Shared/Dtos/General/GeneralDto.cs`:
```csharp
public class GeneralDto
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}
public class GeneralDto<T> : GeneralDto
{
    public T? Data { get; set; }
}
```

---

## 4. La cuestión del Tenant (Multi-tenancy por Email)

Este es uno de los mecanismos más importantes del proyecto. Cada usuario solo ve y opera sobre sus propios proyectos.

### 3.1. Identificación del Tenant
El `TenantId` se determina en el constructor de `AppDbContext`:

```csharp
public AppDbContext(DbContextOptions<AppDbContext> options, IAppCache cache, IHttpContextAccessor httpContextAccessor) : base(options)
{
    _tenantId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? "default";
}
```

**Significado**: El email del usuario autenticado (del JWT) es su `TenantId`.

### 3.2. Entidades con Tenant
No todas las entidades tienen tenant. Solo las que implementan `ITennant` Y tienen `IsTennanted = true`:

```csharp
// Interfaces (Server/Interfaces/IEntity.cs)
public interface IEntity : ISoftDeletable
{
    Guid Id { get; set; }
    bool IsTennanted { get; }        // Propiedad de solo lectura
}

public interface ITennant
{
    string TenantId { get; set; }
}

// Clase base
public abstract class Entity : IEntity
{
    public virtual bool IsTennanted => false;  // Por defecto, NO es tenanted
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOnUtc { get; set; }
}
```

Ejemplo de entidad que SÍ tiene tenant:
```csharp
public class Project : Entity, ITennant
{
    public string TenantId { get; set; } = string.Empty;
    public override bool IsTennanted => true;  // Sobrescribe a true
}
```

### 3.3. Filtros globales de EF Core (Tenant + Soft Delete)
En `OnModelCreating`, se aplican dos filtros globales:

1. **Tenant para Project**: Cada entidad `Project` tiene un solo `HasQueryFilter` (EF Core solo permite uno por entidad). Por eso se combinan ambas condiciones:

```csharp
void TenantedQueryFilter(ModelBuilder builder)
{
    // Project combina soft delete + tenant en un solo filtro
    builder.Entity<Project>().HasQueryFilter(x => !x.IsDeleted && x.TenantId == _tenantId);
}
```

2. **Soft Delete universal para todas las demás entidades**: Se itera automáticamente sobre todas las entidades del modelo. Si tienen la propiedad `IsDeleted` (todas las que heredan de `Entity`), se les aplica un filtro que oculta los registros borrados:

```csharp
void SoftDeleteQueryFilter(ModelBuilder builder)
{
    foreach (var entityType in builder.Model.GetEntityTypes())
    {
        // Excluir Project (ya tiene su filtro combinado) y entidades sin IsDeleted
        if (entityType.ClrType == typeof(Project) || entityType.FindProperty("IsDeleted") == null)
            continue;

        var method = typeof(AppDbContext)
            .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(entityType.ClrType);
        method.Invoke(this, new object[] { builder });
    }
}

private void SetSoftDeleteFilter<TEntity>(ModelBuilder builder) where TEntity : class, ISoftDeletable
{
    builder.Entity<TEntity>().HasQueryFilter(x => !x.IsDeleted);
}
```

**Resultado**: `await _context.Projects.ToListAsync()` traduce a:
```sql
SELECT ... FROM Projects WHERE IsDeleted = 0 AND TenantId = 'usuario@email.com'
```

Y para cualquier otra entidad (ej: `Brands`):
```sql
SELECT ... FROM Brands WHERE IsDeleted = 0
```

### 3.4. Asignación automática al crear
En `SaveChangesAsync`, si una entidad es nueva (`Added`) y es tenanted, se le asigna el `TenantId` automáticamente:

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entities = ChangeTracker.Entries<IEntity>()
        .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).ToList();

    foreach (var row in entities)
    {
        if (row.State == EntityState.Added)
        {
            row.Entity.CreatedOn = DateTime.Now;
            if (row.Entity.IsTennanted)
            {
                var entity = row.Entity as ITennant;
                entity!.TenantId = _tenantId;
            }
        }
    }
    return await base.SaveChangesAsync();
}
```

### 3.5. Caché y Tenant
El `AppDbContext` tiene un sistema de caché personalizado:

```csharp
public async Task<T?> GetOrAddCacheAsync<T>(Func<Task<T?>> addItemFactory, string key, bool isTenanted = false) where T : class
{
    var tenantPart = isTenanted ? $"-{_tenantId}" : "";
    var finalKey = $"{key}{tenantPart}";
    return await _cache.GetOrAddAsync(finalKey, addItemFactory);
}
```

- `isTenanted = false`: La misma caché para todos (ej: listado de Brands globales, o en este proyecto `GetAllProjectDashBoards`).
- `isTenanted = true`: La caché incluye el email en la clave, separando por usuario.

**Nota importante**: Actualmente `GetAllProjectDashBoards` usa `isTenanted = false`, pero el `QueryFilter` de EF Core ya filtra por tenant a nivel de base de datos. Esto significa que la caché es compartida, pero el contenido devuelto a cada usuario es diferente porque la consulta SQL incluye `WHERE TenantId = @email`.

### 3.6. Soft Delete universal
Todas las entidades heredan `Entity`, que implementa `ISoftDeletable`:

```csharp
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedOnUtc { get; set; }
}
```

Al "borrar" algo, nunca se hace `_context.Remove()`. Se hace `row.IsDeleted = true` y luego `SaveChangesAsync()`.

Los registros borrados **no desaparecen** de la base de datos; solo se ocultan automáticamente gracias al `SoftDeleteQueryFilter` aplicado globalmente en `AppDbContext` (ver sección 3.3).

---

## 5. Autenticación y autorización

### 4.1. Flujo de Login
1. Cliente envía email+password a `POST api/authorization/login` (Controller, `[AllowAnonymous]`).
2. Servidor valida con `UserManager`, genera JWT con claims: `Name`, `Email`, `NameIdentifier` (userId), `Role`.
3. Devuelve `LoginResponse { Token, RefreshToken, Expiration, UserName }`.
4. Cliente guarda `Token` en `localStorage` bajo clave `accessToken`.

### 4.2. Envío del token en cada petición
- **Handler**: `AuthenticationHeaderHandler` (DelegatingHandler) lee `localStorage`, inyecta `Authorization: Bearer <token>`.
- **Registro**: Está conectado al `HttpClient` factory como `HttpMessageHandler`.

### 4.3. Validación en el servidor
- **JWT config**: `AddJwtBearer` con `ValidIssuer`, `ValidAudience`, `IssuerSigningKey`.
- **RequireHttpsMetadata = false** (solo en desarrollo, para permitir HTTP localhost).
- **ClockSkew = 5 minutos** (evita rechazos por diferencia de reloj).
- **DefaultPolicy**: Forzada al esquema `Bearer` explícito para evitar que `AddIdentity` cambie a cookies.

### 4.4. Manejo de 401 en el cliente
Si el servidor devuelve 401 (token inválido, expirado, corrupto):

```csharp
// HttpInterceptorService.cs
private async Task InterceptResponseAsync(object sender, HttpClientInterceptorEventArgs e)
{
    if (e.Response?.StatusCode == HttpStatusCode.Unauthorized && !_isRedirecting)
    {
        _isRedirecting = true;
        await _localStorage.RemoveItemAsync("accessToken");
        ((AuthProvider)_authStateProvider).NotifyUserLogout();
        _navigation.NavigateTo("/login", forceLoad: true);
    }
}
```

Esto hace que el usuario nunca se quede "atrapado" viendo su nombre arriba a la derecha mientras todo falla.

---

## 6. Patrones del cliente

### 5.1. HttpServices (comunicación con API)
Servicio centralizado que abstrae **todas** las llamadas HTTP:

```csharp
// Nomenclatura: el nombre del tipo = nombre del endpoint
var endpoint = request.GetType().Name; // ej: "DeleteProject"
var response = await _httpClient.PostAsJsonAsync(endpoint, request);
```

**Características**:
- Siempre usa `POST`, incluso para obtener datos.
- Muestra Snackbar automático en éxito/error via `ISnackBarService`.
- En errores HTTP, devuelve una instancia vacía del tipo de respuesta (no lanza excepciones).
- Método separado `PostForValidationAsync` para validaciones de FluentValidation (no muestra Snackbar).

### 5.2. Pub/Sub con ProjectNotificationService
Servicio singleton en memoria para que componentes hermanos se enteren de cambios:

```csharp
public class ProjectNotificationService
{
    public event Func<Task> OnProjectsChanged = () => Task.CompletedTask;
    public void NotifyProjectsChanged() => _ = OnProjectsChanged().ContinueWith(...);
}
```

Ejemplo de uso: cuando `CreateProjectDialog` crea un proyecto, llama `NotifyProjectsChanged()`. `ProjectList` está suscrito y recarga su lista automáticamente.

### 5.3. AuthProvider
`AuthenticationStateProvider` personalizado que:
- Lee `accessToken` de `localStorage`.
- Parsea el JWT manualmente para extraer claims.
- Expone `NotifyUserAuthentication()` y `NotifyUserLogout()`.

---

## 7. Problemas críticos resueltos en sesiones previas

### 6.1. TenantId siempre era "default"
- **Causa**: Minimal APIs no tenían `RequireAuthorization()`. El middleware JWT no validaba el token, así que `HttpContext.User` estaba vacío.
- **Fix**: Agregado `RequireAuthorization()` a todos los Minimal APIs via `MapGroup`.

### 6.2. 401 después de proteger endpoints
- **Causa**: `RequireHttpsMetadata = true` bloqueaba validación de tokens por HTTP en localhost.
- **Fix**: Cambiado a `false` en desarrollo.

### 6.3. Usuario "aparentemente logueado" pero servidor rechazaba peticiones
- **Causa**: Cliente parseaba token localmente pero no detectaba 401 del servidor.
- **Fix**: Creado `HttpInterceptorService` para interceptar 401 globalmente, borrar token y redirigir a login.

### 6.4. DeleteProject implementado con filtros globales
- Endpoint `DeleteProject` en `ProjectEndPoint.cs` (soft delete + invalidación de caché).
- Botón 🗑️ en `ProjectList.razor` con confirmación y recarga automática.
- Filtros globales de EF Core agregados: `SoftDeleteQueryFilter` oculta automáticamente registros borrados (`IsDeleted = true`) en todas las entidades, y `Project` combina soft delete + tenant en un solo filtro.

---

## 8. Próximas tareas pendientes
- [x] **Soft Delete global implementado** (`SoftDeleteQueryFilter` + `TenantedQueryFilter` combinados).
- [ ] Verificar que el soft-delete de proyectos no rompa cascadas con entidades hijas si algún día se necesita hard-delete.
- [ ] Revisar si otros endpoints que modifican proyectos (ej: approve, editar nombre) invalidan correctamente `GetAllProjectDashBoards`.
- [ ] Agregar roles/permisos si en el futuro hay más de un rol (actualmente solo `User`).
- [ ] Agregar paginación o búsqueda en `ProjectList` si la lista crece mucho.

---

## 9. Principios de programación aplicados en este proyecto

Además de SOLID, DRY, KISS, YAGNI y Design Patterns, aquí hay principios adicionales que aplicamos:

| Principio | Qué significa | Aplicación en ProjectToolV-2 |
|-----------|---------------|------------------------------|
| **GRASP** | Patrones para asignar responsabilidades a clases. | `HttpInterceptorService` separado de `AuthProvider`. |
| **Law of Demeter** | No hables con extraños. | `AppDbContext` no accede a `HttpContext.User.Claims` directamente; usa `IHttpContextAccessor`. |
| **Fail Fast** | Validar inputs lo antes posible. | Endpoints validan DTOs antes de tocar la base de datos. |
| **Defensive Programming** | Asumir que todo puede fallar. | Proteger divisiones por cero, validar arrays vacíos, usar `?.` y `??`. |
| **Null-Safety** | Escribir código que no rompa por nulls. | `ArgumentNullException.ThrowIfNull()`, `Result<T>` en lugar de retornar null. |
| **Tell, Don't Ask** | Decirle al objeto qué hacer, no preguntar su estado. | `_context.InvalidateCache()` en lugar de manipular el diccionario externamente. |
| **Single Source of Truth** | Cada dato existe en un solo lugar. | `TenantId` se lee una vez en `AppDbContext` y se usa en todo el request. |
| **Command-Query Separation (CQS)** | Un método es comando o query, no ambos. | `SaveChangesAsync()` (comando) vs `GetOrAddCacheAsync()` (query). |
| **Convention over Configuration** | Seguir convenciones del framework. | `ApplyConfigurationsFromAssembly`, `AddEndPoints` por reflexión. |
| **Separation of Concerns** | Cada módulo maneja una sola preocupación. | `Server` = API + persistencia; `Client` = UI; `Shared` = DTOs. |
| **Least Astonishment** | El código se comporta como se espera. | `Delete` hace soft delete (`IsDeleted = true`), no borra físico. |
| **Boy Scout Rule** | Dejar el código más limpio de lo que lo encontraste. | Cada bug que arreglamos, también limpiamos código muerto cercano. |
| **Pareto Principle** | 80% del valor viene del 20% del esfuerzo. | Priorizar los 3 bugs críticos (tenant, auth, 401) antes de los 15 menores. |

### 9.1. Guía de Null-Safety y Programación Defensiva

| Técnica | Qué es | Ejemplo en C# |
|---------|--------|---------------|
| **Guard Clauses** | Validar al inicio del método y retornar/salir inmediatamente. | `if (dto is null) return Result.Fail("DTO nulo");` |
| **Null-Conditional (`?.`)** | Acceder a propiedades solo si el objeto no es null. | `var email = user?.FindFirst(ClaimTypes.Email)?.Value ?? "default";` |
| **Null-Coalescing (`??`)** | Proveer valor por defecto si algo es null. | `var tenantId = email ?? "default";` |
| **`ArgumentNullException.ThrowIfNull()`** | .NET 6+ — lanza excepción clara si parámetro es null. | `ArgumentNullException.ThrowIfNull(dto);` |
| **Result<T> Pattern** | Devolver éxito/fallo con mensaje en lugar de null o excepciones. | `Result<List<ProjectDto>> GetAll()` → `Result.Fail("No autorizado")`. |
| **Fail Fast con mensajes claros** | Si algo es imposible de recuperar, fallar inmediatamente con contexto. | `throw new InvalidOperationException($"Project {id} no encontrado");` |
| **Evitar `throw new Exception`** | Nunca lanzar la excepción base. Usar tipos específicos. | `ArgumentException`, `InvalidOperationException`, `UnauthorizedAccessException`. |

---

## 10. Lección aprendida: Riesgo de librerías de UI de terceros

> **Fecha**: 2026-06-29
>
> **Contexto**: Al actualizar MudBlazor de v9.x a v9.6 en este proyecto, **11 archivos se rompieron** y dejaron de compilar:
> - Métodos renombrados (`ShowMessageBox` → `ShowMessageBoxAsync`).
> - Propiedades eliminadas (`AutoGrow` reemplazado por `Sizing`, `PanelClass` renombrado a `TabPanelsClass`).
> - Cambios en reglas de compilación de Blazor (ambigüedad de `context` en `ActivatorContent`).
>
> **Conclusión**: Las librerías de UI de terceros (MudBlazor, Bootstrap, etc.) pueden romper APIs en actualizaciones menores. En proyectos donde la UI crítica es custom (canvas SVG, equipos técnicos, animaciones), es preferible mantener HTML+CSS manual para el núcleo visual, y usar componentes de terceros **solo donde aportan valor real** (diálogos, tablas complejas, notificaciones, forms).
>
> **Impacto en este proyecto**: `ProjectToolV-2` usa MudBlazor extensivamente como framework UI principal, lo cual es correcto para una app de gestión de proyectos (tablas, forms, layouts). Sin embargo, cada actualización de MudBlazor requiere revisión de todos los componentes `.razor` que usan sus APIs.

---

## 11. Instrucciones para retomar conversación

Cuando se retome este proyecto en una nueva sesión, decir explícitamente:

```
Abre el proyecto en C:\Programas\ProjecToolV-2 y lee el archivo .opencode/context.md para contexto.
```

O copiar/pegue la sección relevante de este archivo en el prompt inicial.

---

## 11. Archivos clave del sistema

### Servidor
| Archivo | Rol |
|---------|-----|
| `Server/Services/RegisterServices.cs` | Registro de DI: EF, Identity, JWT, CORS, Endpoints, Cache |
| `Server/Services/AppBuilder.cs` | Pipeline HTTP + mapeo automático de `IEndPoint` |
| `Server/DataContext/AppDbContext.cs` | DbContext + Tenant + SoftDelete + Cache |
| `Server/DataContext/IAppDbContext.cs` | Interfaz del DbContext para inyección en endpoints |
| `Server/Interfaces/IEntity.cs` | Contratos base: `IEntity`, `ISoftDeletable`, `ITennant`, `Entity` |
| `Server/Interfaces/EndPoints/IEndPoint.cs` | Interfaz que todas las clases de endpoints implementan |
| `Server/Controllers/AuthorizationController.cs` | Login, Register, ChangePassword (único Controller) |

### Cliente
| Archivo | Rol |
|---------|-----|
| `CllientMudBlazor/Program.cs` | Registro de DI: HttpClient, MudBlazor, Auth, Interceptor |
| `CllientMudBlazor/Services/HttpServices.cs` | Servicio centralizado de llamadas HTTP |
| `CllientMudBlazor/Services/AuthenticationHeaderHandler.cs` | Inyecta Bearer token en cada request |
| `CllientMudBlazor/Services/HttpInterceptorService.cs` | Manejo global de 401 |
| `CllientMudBlazor/Services/AuthProvider.cs` | AuthenticationStateProvider personalizado |
| `CllientMudBlazor/Services/ProjectNotificationService.cs` | Pub/sub en memoria entre componentes |
| `CllientMudBlazor/Layout/MainLayout.razor` | Layout principal con `AuthorizeView` y `ProjectList` |
| `CllientMudBlazor/Pages/MainDashBoards/ProjectList.razor` | Lista lateral de proyectos |
| `CllientMudBlazor/App.razor` | Router principal con cascada de autenticación |

### Shared
| Archivo | Rol |
|---------|-----|
| `Shared/Dtos/General/GeneralDto.cs` | DTO base de respuesta para todos los endpoints |
| `Shared/Dtos/Projects/ProjectDashboardDto.cs` | DTOs de proyectos: `GetAllProjectDashBoards`, `DeleteProject`, etc. |
