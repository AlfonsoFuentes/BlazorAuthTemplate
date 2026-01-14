using Azure.Core;
using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Dtos.Projects.Plannings.Gantts;
using Shared.Enums;

namespace Server.EndPoints.ProjectDashBoard.ProjectPlannings.Communications
{

    public class CommunicationEndpoints : IEndPoint
    {
        public static string[] CacheKeys(Guid projectId, Guid taskId) => new[]
        {           $"{typeof(GetAllGanttTasks).Name}-{projectId}",
                    $"GetAllCommunicationToValidateName-{projectId}",
                    $"{typeof(GetAllProjectCommunications).Name}-{projectId}",
                    $"{typeof(GetCommunicationById).Name}-{taskId}",
                    $"{typeof(GetAllProjectDashBoards).Name}",
                    $"{typeof(GetProjectDashBoardStartById).Name}-{projectId}",
                    $"{typeof(ExportProjectPlannPDF).Name}-{projectId}"
    };
        // ---------------------------------------------------------
        // 🛠️ MAPPERS (Para mantener limpios los endpoints)
        // ---------------------------------------------------------

        // Mapeo DTO -> Entidad (Solo propiedades simples)
        void MapFromDto(CommunicationDto dto, Communication row)
        {
            row.Name = dto.Name;
            row.Type = dto.Type;
            row.Artifact = dto.Artifact;
            row.Trigger = dto.Trigger;
            row.LinkedGanttTaskId = dto.SelectedGanttTask?.Id;
            row.DaysOffsetOrFrequency = dto.DaysOffsetOrFrequency;
            // Nota: Receivers se maneja en el endpoint por requerir acceso a BD
        }

        // Mapeo Entidad -> DTO
        static CommunicationDto MapToDto(Communication row)
        {
            return new CommunicationDto
            {
                Id = row.Id,
                ProjectId = row.ProjectId,
                Name = row.Name,
                Type = row.Type,
                Artifact = row.Artifact,
                Trigger = row.Trigger,
              
                SelectedGanttTask = row.LinkedGanttTask == null ? null : new GanttDto
                {
                    Id = row.LinkedGanttTask.Id,
                    Name = row.LinkedGanttTask.Name,
                    
                    StartDate = row.LinkedGanttTask.StartDate,
                    EndDate = row.LinkedGanttTask.EndDate
                },
                DaysOffsetOrFrequency = row.DaysOffsetOrFrequency,
                // Convertimos la relación Many-to-Many a DTOs simples
                Receivers = row.Receivers.Select(r => new StakeHolderSimpleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Role = r.RoleInsideProject?.Name ?? ""
                }).ToList()
            };
        }

        // ---------------------------------------------------------
        // 🛣️ ENDPOINTS MAPPING
        // ---------------------------------------------------------
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ==========================================
            // 📝 CREATE
            // ==========================================
            app.MapPost("CreateCommunication", async (CreateCommunication dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                // 1. Validaciones de Negocio
                if (dto.Trigger != CommunicationTrigger.Periodic && dto.SelectedGanttTask == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "A linked Gantt Task is required for this trigger type." });

                // 2. Obtener Stakeholders reales desde los IDs del DTO
                var selectedIds = dto.Receivers.Select(x => x.Id).ToList();
                var receivers = await _context.StakeHolders.Where(s => selectedIds.Contains(s.Id)).ToListAsync();

                if (!receivers.Any())
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "At least one receiver is required." });

                // 3. Crear Entidad
                var row = new Communication
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                    Receivers = receivers // EF Core gestiona la tabla intermedia
                };

                MapFromDto(dto, row);

                // 4. Gestión de Orden (Igual que Deliverable)
                var cacheKeyAll = $"{typeof(GetAllProjectCommunications).Name}-{dto.ProjectId}";
                var maxOrder = await getNextOrder.GetNextOrderAsync<Communication>(cacheKeyAll, dto.ProjectId);

                // Asumiendo que agregaste la propiedad 'Order' a la entidad Communication
                 row.Order = maxOrder; 

                await _context.Communications.AddAsync(row);

                // 5. Guardar y Limpiar Caché
                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(row.ProjectId, row.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Communication created." });
            });

            // ==========================================
            // 📝 UPDATE
            // ==========================================
            app.MapPost("UpdateCommunication", async (UpdateCommunication dto, IAppDbContext _context) =>
            {
                // 1. Cargar con Relaciones (Include es vital)
                var row = await _context.Communications
                    .Include(c => c.Receivers)
                    .FirstOrDefaultAsync(c => c.Id == dto.Id);

                if (row == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Communication not found." });

                // 2. Actualizar Propiedades Simples
                MapFromDto(dto, row);

                // 3. Actualizar Relación Many-to-Many
                // Extraemos IDs de la lista de objetos heredada (CommunicationDto.Receivers)
                var selectedIds = dto.Receivers.Select(x => x.Id).ToList();

                // Limpiamos la lista actual y recargamos los seleccionados
                row.Receivers.Clear();
                var newReceivers = await _context.StakeHolders.Where(s => selectedIds.Contains(s.Id)).ToListAsync();

                foreach (var r in newReceivers)
                {
                    row.Receivers.Add(r);
                }

                // 4. Guardar y Limpiar Caché
                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(row.ProjectId, row.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Communication updated." });
            });

            // ==========================================
            // 🗑️ DELETE
            // ==========================================
            app.MapPost("DeleteCommunication", async (DeleteCommunication dto, IAppDbContext _context) =>
            {
                var row = await _context.Communications.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Communication not found." });

                row.IsDeleted = true;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    //Reordenar elementos restantes(Lógica de Deliverable)
                     var datas = await _context.Communications
                                              .Where(x => x.ProjectId == dto.ProjectId)
                                              .OrderBy(x => x.Order) // Asumiendo propiedad Order
                                              .ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i++;
                    }
            


                    await _context.SaveChangesAsync();
                    var keys = CacheKeys(row.ProjectId, row.Id);
                    _context.InvalidateCache(keys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Suceeded delete communication." });
                }

                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Failed to delete communication." });
            });

            // ==========================================
            // 🔍 GET ALL (Cached)
            // ==========================================
            app.MapPost("GetAllProjectCommunications", async (GetAllProjectCommunications dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllProjectCommunications).Name}-{dto.ProjectId}";

                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Communications
                        .AsSplitQuery()
                        .AsNoTracking()
                        .Where(x => x.ProjectId == dto.ProjectId)
                        .Include(x => x.LinkedGanttTask)
                        .Include(x => x.Receivers).ThenInclude(r => r.RoleInsideProject)
                        // .OrderBy(x => x.Order) // Descomentar si usas Order
                        .OrderBy(x => x.Trigger) // Orden fallback
                        .ToListAsync();
                }, cacheKey);

                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<CommunicationDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });

            // ==========================================
            // 🔍 GET BY ID (Cached)
            // ==========================================
            app.MapPost("GetCommunicationById", async (GetCommunicationById dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetCommunicationById).Name}-{dto.Id}";

                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Communications
                        .AsSplitQuery()
                        .AsNoTracking()
                        .Include(x => x.LinkedGanttTask)
                        .Include(x => x.Receivers).ThenInclude(r => r.RoleInsideProject)
                        .FirstOrDefaultAsync(x => x.Id == dto.Id);
                }, cacheKey);

                if (row == null)
                    return Results.Ok(new GeneralDto<CommunicationDto> { Succeeded = false, Message = "Communication not found." });

                return Results.Ok(new GeneralDto<CommunicationDto>
                {
                    Succeeded = true,
                    Data = MapToDto(row)
                });
            });

            // ==========================================
            // ✅ VALIDATE NAME (Unique)
            // ==========================================
            app.MapPost("ValidateCommunicationName", async (ValidateCommunicationName dto, IAppDbContext _context) =>
            {
                // Reutilizamos la caché de GetAll para validar rápido sin ir a BD
                var cacheKeyAll = $"GetAllCommunicationToValidateName--{dto.ProjectId}";

                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Communications
                        .AsNoTracking()
                        .Where(x => x.ProjectId == dto.ProjectId)
                        .ToListAsync();
                }, cacheKeyAll);

                // Predicado: Si es nuevo, busca nombre igual. Si es edición, busca nombre igual pero ID diferente.
                Func<Communication, bool> predicate = x =>
                    dto.Id == Guid.Empty
                        ? x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)
                        : x.Id != dto.Id && x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase);

                var exists = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = exists, // True = Duplicado (Error), False = Disponible
                    Message = exists ? "Name already in use." : "Name is available."
                };
            });

            // ==========================================
            // ↕️ CHANGE ORDER (Drag & Drop)
            // ==========================================
            /* Descomentar cuando agregues la propiedad Order a la entidad y DTO   */
            app.MapPost("ChangeOrderCommunication", async (ChangeOrderCommunication dto, IAppDbContext _context) =>
            {
                var currentRow = await _context.Communications.FindAsync(dto.Id);
                if (currentRow == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found." });

                var targetRow = await _context.Communications
                    .Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder)
                    .FirstOrDefaultAsync();

                if (targetRow != null)
                {
                     //Intercambio de orden
                    targetRow.Order = currentRow.Order;
                    currentRow.Order = dto.NewOrder;

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var cacheKeyAll = $"{typeof(GetAllProjectCommunications).Name}-{dto.ProjectId}";
                        _context.InvalidateCache(cacheKeyAll);

                        return Results.Ok(new GeneralDto { Succeeded = true, Message = "Order changed." });
                    }
                }

                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Failed to change order." });
            });
           /* */
        }
    }
    
}
