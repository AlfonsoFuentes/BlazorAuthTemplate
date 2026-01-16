using Server.DataContext;
using Server.Interfaces.EndPoints;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.General;
using Shared.Dtos.Projects._1._Starts.Hazops;
using Shared.Enums.DashBoardTable;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Hazops
{
    public class HazopEndPoint : IEndPoint
    {
        static HazopNodeDto MapToDto(HazopNode row)
        {
            var dto = new HazopNodeDto
            {
                Id = row.Id,
                ProjectId = row.ProjectId,
                Order = row.Order,
                Name = row.Name,
                Description = row.Description,
                DesignIntent = row.DesignIntent,
                Details = row.Details.Select(d => new HazopDetailDto
                {
                    Id = d.Id,
                    HazopNodeId = d.HazopNodeId,
                    Parameter = d.Parameter,
                    GuideWord = d.GuideWord,
                    Causes = d.Causes,
                    Consequences = d.Consequences,
                    Safeguards = d.Safeguards,
                    Recommendations = d.Recommendations,
                    ProjectId = row.ProjectId,
                    Order = row.Order,
                }).ToList()
            };


            return dto;
        }
        // Mapeo Base
        void MapFromDto(HazopNodeDto dto, HazopNode row)
        {
            row.Name = dto.Name;
            row.Description = dto.Description;
            row.DesignIntent = dto.DesignIntent;
        }

        // Mapeo con Hijos (Create/Edit)
       

        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // --- CREATE ---
            app.MapPost("CreateHazopNode", async (CreateHazopNode dto, IAppDbContext _context) =>
            {
                var row = new HazopNode
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId
                };

                MapFromDto(dto, row);
                
                // Cálculo de orden
                var maxOrder = await _context.Set<HazopNode>()
                    .Where(x => x.ProjectId == dto.ProjectId)
                    .MaxAsync(x => (int?)x.Order) ?? 0;
                row.Order = maxOrder + 1;

                await _context.Set<HazopNode>().AddAsync(row);
                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                if (await _context.SaveChangesAsync() > 0)
                {
                    // Invalidación usando tu ProjectCacheBrain
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Hazop);
                    _context.InvalidateCache(keys);
                    dto.Id = row.Id;
                    return Results.Ok(new GeneralDto<HazopNodeDto> { Succeeded = true, Message = "Hazop Node created.",Data=dto });
                }
                return Results.Ok(new GeneralDto<HazopNodeDto> { Succeeded = false, Message = "Error creating Hazop Node." });
            });
            app.MapPost("EditHazopNode", async (EditHazopNode dto, IAppDbContext _context) =>
            {
                var row = await _context.HazopNodes.FindAsync(dto.Id);
                if (row == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Hazop Analisys not found."
                    });
                }
                MapFromDto(dto, row);

                // Cálculo de orden

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                if (await _context.SaveChangesAsync() > 0)
                {
                    // Invalidación usando tu ProjectCacheBrain
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Hazop);
                    _context.InvalidateCache(keys);

                    return Results.Ok(new GeneralDto<HazopNodeDto> { Succeeded = true, Message = "Hazop Node created.",Data=dto });
                }
                return Results.Ok(new GeneralDto<HazopNodeDto> { Succeeded = false, Message = "Error editing Hazop Node." });
            });
            // --- GET BY ID ---
            app.MapPost("GetHazopNodeById", async (GetHazopNodeById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetHazopNodeById).Name}-{request.Id}";

                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Set<HazopNode>()
                        // ✅ Incluimos los detalles (hijos) del nodo
                        .Include(x => x.Details)
                        .Where(x => x.Id == request.Id)
                        .AsSplitQuery() // Recomendado por EF Core al usar Includes de colecciones
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                }, cacheKey);

                if (row == null)
                {
                    return Results.Ok(new GeneralDto<HazopNodeDto>
                    {
                        Succeeded = false,
                        Message = "Hazop Node not found."
                    });
                }

                // Mapeo manual de Entidad a DTO
                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<HazopNodeDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });
            // --- GET ALL ---
            app.MapPost("GetAllHazopNodes", async (GetAllHazopNodes dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllHazopNodes).Name}-{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Set<HazopNode>()
                        .Include(x => x.Details)
                        .Where(x => x.ProjectId == dto.ProjectId)
                        .OrderBy(x => x.Order)
                        .AsNoTracking()
                        .ToListAsync();
                }, cacheKey);

                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<HazopNodeDto>> { Succeeded = true, Data = dtos });
            });

            // --- DELETE ---
            app.MapPost("DeleteHazopNode", async (DeleteHazopNode dto, IAppDbContext _context) =>
            {
                var row = await _context.Set<HazopNode>().FindAsync(dto.Id);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Hazop Node not found" });

                row.IsDeleted = true; // Borrado lógico como tu RiskMatrix

                if (await _context.SaveChangesAsync() > 0)
                {
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Hazop);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Hazop node fully removed!" });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Hazop node to delete something went wrong" });
            });
            // --- CREATE DETAIL ---
            app.MapPost("CreateHazopDetailNode", async (CreateHazopDetailNode dto, IAppDbContext _context) =>
            {
                var detail = new HazopDetail
                {
                    Id = Guid.NewGuid(),
                    HazopNodeId = dto.HazopNodeId,
                    Parameter = dto.Parameter,
                    GuideWord = dto.GuideWord,
                    Causes = dto.Causes,
                    Consequences = dto.Consequences,
                    Safeguards = dto.Safeguards,
                    Recommendations = dto.Recommendations
                };

                await _context.Set<HazopDetail>().AddAsync(detail);

                if (await _context.SaveChangesAsync() > 0)
                {
                    // Invalidamos el cache del Nodo padre para que al refrescar traiga el nuevo detalle

                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, dto.HazopNodeId, DashBoardsStartTable.Hazop);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Detail added successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Error adding detail." });
            });

            // --- EDIT DETAIL ---
            app.MapPost("EditHazopDetailNode", async (EditHazopDetailNode dto, IAppDbContext _context) =>
            {
                var detail = await _context.Set<HazopDetail>().FindAsync(dto.Id);
                if (detail == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Detail not found." });

                detail.Parameter = dto.Parameter;
                detail.GuideWord = dto.GuideWord;
                detail.Causes = dto.Causes;
                detail.Consequences = dto.Consequences;
                detail.Safeguards = dto.Safeguards;
                detail.Recommendations = dto.Recommendations;

                if (await _context.SaveChangesAsync() > 0)
                {
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, dto.HazopNodeId, DashBoardsStartTable.Hazop);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Detail updated." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });

            // --- GET DETAIL BY ID ---
            app.MapPost("GetHazopDetailById", async (GetHazopDetailById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetHazopDetailById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Set<HazopDetail>()
                        .Where(x => x.Id == request.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                }, cacheKey);

                if (row == null) return Results.Ok(new GeneralDto<HazopDetailDto> { Succeeded = false });

                var dto = new HazopDetailDto
                {
                    Id = row.Id,
                    HazopNodeId = row.HazopNodeId,
                    Parameter = row.Parameter,
                    GuideWord = row.GuideWord,
                    Causes = row.Causes,
                    Consequences = row.Consequences,
                    Safeguards = row.Safeguards,
                    Recommendations = row.Recommendations
                };

                return Results.Ok(new GeneralDto<HazopDetailDto> { Succeeded = true, Data = dto });
            });

            // --- DELETE DETAIL ---
            app.MapPost("DeleteHazopDetailNode", async (DeleteHazopDetailNode dto, IAppDbContext _context) =>
            {
                var detail = await _context.Set<HazopDetail>().FindAsync(dto.Id);
                if (detail == null) return Results.Ok(new GeneralDto { Succeeded = false });

                // Borrado lógico siguiendo tu estándar
                detail.IsDeleted = true;

                if (await _context.SaveChangesAsync() > 0)
                {
                    // Al borrar un detalle, debemos invalidar el nodo padre para que la lista en UI se actualice

                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, dto.HazopNodeId, DashBoardsStartTable.Hazop);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Detail deleted." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });
            app.MapPost("ValidateHazopName", async (ValidateHazopName dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllHazopNodes).Name}-{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.HazopNodes.AsNoTracking()
                        .Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                }, cacheKey);

                Func<HazopNode, bool> predicate = x =>
                   
                   (dto.Id == Guid.Empty
                       ? x.Name.Trim().Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                       : x.Id != dto.Id && x.Name.Trim().Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase));

                var isUnique = !rows!.Any(predicate);
                return new GeneralDto<bool> { Succeeded = true, Data = isUnique, Message = isUnique ? "Name is available." : "Name already in use." };
            });
        }
    }
}
