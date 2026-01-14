using Microsoft.EntityFrameworkCore;
using Server.DataContext;
using Server.Domain.CommonEntities.ProjectManagements;
using Server.Interfaces.EndPoints;
using Shared.Dtos;
using Shared.Dtos.General;
using Shared.Dtos.ProjectDefinitions;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.ExpertJudgements;
using Shared.Enums;
using Shared.Enums.DashBoardTable;
using Shared.Enums.ProjectDefinitionTypes;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.ProjectDefinitionItems
{
    public class ProjectDefinitionEndPoints : IEndPoint
    {
        // Mapper manual optimizado
        static void MapFromDto(ProjectDefinitionItemDto dto, ProjectDefinitionItem row)
        {
            row.Name = dto.Name;
            row.Description = dto.Description;
            // No mapeamos ProjectId ni Type en edición para evitar cambios accidentales de estructura
        }

        static ProjectDefinitionItemDto MapToDto(ProjectDefinitionItem row)
        {
            return new ProjectDefinitionItemDto
            {
                Id = row.Id,
                ProjectId = row.ProjectId,
                Name = row.Name,
                Description = row.Description,
                Order = row.Order,
                Type = row.Type
            };
        }

        public void MapEndPoint(IEndpointRouteBuilder app)
        {


            // ✅ CREATE: Sirve para cualquier tipo (Objetivo, Alcance, etc.)
            app.MapPost("CreateProjectDefinitionItem", async (CreateProjectDefinitionItem dto, IAppDbContext _context) =>
            {
                var row = new ProjectDefinitionItem
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                    Name = dto.Name,
                    Description = dto.Description,
                    Type = dto.Type // <--- Aquí se define qué es
                };

                await _context.ProjectDefinitionItems.AddAsync(row);

                // Actualizar fecha modificación del Proyecto
                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null) project.LastModifiedOn = DateTime.UtcNow;

                // Lógica de Orden: Max(Order) filtrado por TIPO
                var maxOrder = await _context.ProjectDefinitionItems
                    .Where(x => x.ProjectId == dto.ProjectId && x.Type == dto.Type)
                    .MaxAsync(x => (int?)x.Order) ?? 0;

                row.Order = maxOrder + 1;

                if (await _context.SaveChangesAsync() > 0)
                {

                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id,  dto.Type);
                    _context.InvalidateCache(keys);
          

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        // El mensaje sale dinámico: "Objective created successfully"
                        Message = $"{dto.Type} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });

            // ✅ EDIT
            app.MapPost("EditProjectDefinitionItem", async (EditProjectDefinitionItem dto, IAppDbContext _context) =>
            {
                var row = await _context.ProjectDefinitionItems.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Item not found." });

                MapFromDto(dto, row); // Actualiza nombre y descripción

                if (await _context.SaveChangesAsync() > 0)
                {
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, dto.Type);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Updated successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Update failed." });
            });

            // ✅ GET BY ID
            app.MapPost("GetProjectDefinitionById", async (GetProjectDefinitionById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetProjectDefinitionById).Name}-{request.Id}";

                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.ProjectDefinitionItems
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == request.Id);
                }, cacheKey);

                if (row == null)
                    return Results.Ok(new GeneralDto<ProjectDefinitionItemDto> { Succeeded = false, Message = "Not found." });

                return Results.Ok(new GeneralDto<ProjectDefinitionItemDto>
                {
                    Succeeded = true,
                    Data = MapToDto(row)
                });
            });

            // ✅ GET ALL (Filtrado por Tipo)
            app.MapPost("GetAllProjectDefinitions", async (GetAllProjectDefinitions dto, IAppDbContext _context) =>
            {
                // La clave de caché INCLUYE el TIPO. Ejemplo: GetAll-ProjectId-Objective
                var cacheKey = $"{typeof(GetAllProjectDefinitions).Name}-{dto.Type}-{dto.ProjectId}";

                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.ProjectDefinitionItems
                        .AsNoTracking()
                        .AsSplitQuery()
                        .AsQueryable()
                        .Where(x => x.ProjectId == dto.ProjectId && x.Type == dto.Type) // Filtro Crítico
                        .OrderBy(x => x.Order)
                        .ToListAsync();
                }, cacheKey);

                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<ProjectDefinitionItemDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });

            // ✅ DELETE
            app.MapPost("DeleteProjectDefinitionItem", async (DeleteProjectDefinitionItem dto, IAppDbContext _context) =>
            {
                var row = await _context.ProjectDefinitionItems.FindAsync(dto.Id);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found" });

                row.IsDeleted = true;

                if (await _context.SaveChangesAsync() > 0)
                {
                    // Reordenar solo los elementos de ese TIPO
                    var remainingItems = await _context.ProjectDefinitionItems
                        .Where(x => x.ProjectId == dto.ProjectId && x.Type == dto.Type)
                        .OrderBy(x => x.Order)
                        .ToListAsync();

                    int i = 1;
                    foreach (var item in remainingItems)
                    {
                        item.Order = i++;
                    }
                    await _context.SaveChangesAsync();

                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, dto.Type);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Deleted successfully" });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Delete failed" });
            });

            // ✅ VALIDATE NAME
            app.MapPost("ValidateProjectDefinitionName", async (ValidateProjectDefinitionName dto, IAppDbContext _context) =>
            {
                // Reutilizamos la caché del GetAll
                var cacheKey = $"{typeof(GetAllProjectDefinitions).Name}-{dto.Type}-{dto.ProjectId}";

                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.ProjectDefinitionItems
                        .AsNoTracking()
                         .AsSplitQuery()
                        .AsQueryable()
                        .Where(x => x.ProjectId == dto.ProjectId && x.Type == dto.Type)
                        .ToListAsync();
                }, cacheKey);

                Func<ProjectDefinitionItem, bool> predicate = x =>
                    dto.Id == Guid.Empty
                    ? x.Name.Trim().Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                    : x.Id != dto.Id && x.Name.Trim().Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase);

                var isUnique = !rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });

            // ✅ CHANGE ORDER
            app.MapPost("ChangeOrderProjectDefinitionItem", async (ChangeOrderProjectDefinitionItem dto, IAppDbContext _context) =>
            {
                var currentRow = await _context.ProjectDefinitionItems.FindAsync(dto.Id);
                if (currentRow == null) return Results.Ok(new GeneralDto { Succeeded = false });

                var targetRow = await _context.ProjectDefinitionItems
                    .Where(x => x.ProjectId == dto.ProjectId
                             && x.Type == dto.Type
                             && x.Order == dto.NewOrder)
                    .FirstOrDefaultAsync();

                if (targetRow == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Target order not found" });

                // Swap
                targetRow.Order = currentRow.Order;
                currentRow.Order = dto.NewOrder;

                if (await _context.SaveChangesAsync() > 0)
                {
                    var cacheKeyAll = $"{typeof(GetAllProjectDefinitions).Name}-{dto.Type}-{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
              
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Reordered successfully" });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });
        }

        // Helper para invalidar cachés específicas y globales
        
    }
}