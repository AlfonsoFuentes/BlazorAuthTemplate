using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Projects._1._Starts.RiskMatrixs.RiskResponseActions;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.RiskResponseActions
{
    public class RiskResponseActionEndPoint : IEndPoint
    {
        void MapFromDto(RiskResponseActionDto dto, RiskResponseAction row)
        {
            row.Description = dto.Description;
            row.AssignedTo = dto.AssignedTo;
            row.DueDate = dto.DueDate;
            row.IsCompleted = dto.IsCompleted;
            row.ActionType = dto.ActionType;
            // RiskMatrixId se asigna en el Create o no se toca en Edit
        }

        static RiskResponseActionDto MapToDto(RiskResponseAction row)
        {
            RiskResponseActionDto dto = new();
            dto.Id = row.Id;
            dto.RiskMatrixId = row.RiskMatrixId;
            dto.Description = row.Description;
            dto.AssignedTo = row.AssignedTo;
            dto.DueDate = row.DueDate;
            dto.IsCompleted = row.IsCompleted;
            dto.ActionType = row.ActionType;
            dto.Order = row.Order;

            return dto;
        }

        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // --- CREATE ---
            app.MapPost("CreateRiskResponseAction", async (CreateRiskResponseAction dto, IAppDbContext _context) =>
            {
                var row = new RiskResponseAction
                {
                    Id = Guid.NewGuid(),
                    RiskMatrixId = dto.RiskMatrixId,
                    // Inicializamos valores por defecto si es necesario
                    IsDeleted = false
                };

                MapFromDto(dto, row);

                await _context.RiskResponseActions.AddAsync(row);

                // Invalidación idéntica a tu ejemplo




                var cacheKeyAll = $"{typeof(GetAllRiskResponseActionsByRiskId).Name}{dto.RiskMatrixId}";
                var maxOrder = await _context.RiskResponseActions
         .Where(x => x.RiskMatrixId == dto.RiskMatrixId && !x.IsDeleted)
         .MaxAsync(x => (int?)x.Order) ?? 0;
                row.Order = maxOrder;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    // Invalidamos caché (Asumiendo que tienes GetRiskMatrixById u otros que dependan de esto)
                    // Ajusta las keys según tus necesidades reales de invalidación
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards);

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(RiskResponseAction).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(RiskResponseAction).Name} was not created successfully."
                });
            });

            // --- EDIT ---
            app.MapPost("EditRiskResponseAction", async (EditRiskResponseAction dto, IAppDbContext _context) =>
            {
                var row = await _context.RiskResponseActions.Include(x => x.RiskMatrix).FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Risk Response Action not found."
                    });

                MapFromDto(dto, row);

                // Actualizar padre si es necesario
                // var riskMatrix = await _context.RiskMatrices.FindAsync(row.RiskMatrixId);
                // if (riskMatrix != null) riskMatrix.LastModifiedOn = DateTime.UtcNow;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {   // Invalidación idéntica a tu ejemplo
                    var cacheKeyExportProjectPlannPDF = $"{typeof(ExportProjectCharterReport).Name}-{row.RiskMatrix.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardStartById).Name}-{row.RiskMatrix.ProjectId}";


                    var cacheKeyAll = $"{typeof(GetAllRiskResponseActionsByRiskId).Name}{row.RiskMatrixId}";
                    var cacheKeyId = $"{typeof(GetRiskResponseActionById).Name}-{dto.Id}";

                    _context.InvalidateCache(cacheKeyAll, cacheKeyId, cacheKeyExportProjectPlannPDF,cacheKeyProjectDashBoards,cacheKeyProjectDashBoardsById);

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(RiskResponseAction).Name} Updated successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true, // A veces se devuelve true aunque no hubo cambios en DB si los datos eran iguales
                    Message = $"{typeof(RiskResponseAction).Name} was not Updated successfully (or no changes detected)."
                });
            });

            // --- GET BY ID ---
            app.MapPost("GetRiskResponseActionById", async (GetRiskResponseActionById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetRiskResponseActionById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.RiskResponseActions
                         .Where(x => x.Id == request.Id)
                         .AsSplitQuery()
                         .AsNoTracking()
                         .AsQueryable()
                         .FirstOrDefaultAsync();
                }, cacheKey);

                if (row == null)
                    return Results.Ok(new GeneralDto<RiskResponseActionDto>
                    {
                        Succeeded = false,
                        Message = "Risk Response Action not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<RiskResponseActionDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // --- GET ALL BY PARENT ID ---
            app.MapPost("GetAllRiskResponseActionsByRiskId", async (GetAllRiskResponseActionsByRiskId dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllRiskResponseActionsByRiskId).Name}{dto.RiskMatrixId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.RiskResponseActions
                          .AsSplitQuery()
                          .AsNoTracking()
                          .AsQueryable()
                          .Where(x => x.RiskMatrixId == dto.RiskMatrixId && !x.IsDeleted) // Filtramos IsDeleted
                          .OrderBy(x => x.Order)
                          .ToListAsync();
                }, cacheKey);

                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<RiskResponseActionDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });

            // --- DELETE ---
            app.MapPost("DeleteRiskResponseAction", async (DeleteRiskResponseAction dto, IAppDbContext _context) =>
            {
                var row = await _context.RiskResponseActions.Include(x=>x.RiskMatrix).FirstOrDefaultAsync(x=>x.Id==dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(RiskResponseAction).Name} was not found"
                    });
                }

                // Borrado lógico
                row.IsDeleted = true;

                if (await _context.SaveChangesAsync() > 0)
                {
                    // Reordenar los elementos restantes
                    var datas = await _context.RiskResponseActions
                        .Where(x => x.RiskMatrixId == row.RiskMatrixId && !x.IsDeleted)
                        .OrderBy(x => x.Order)
                        .ToListAsync();

                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await _context.SaveChangesAsync();
                    var cacheKeyExportProjectPlannPDF = $"{typeof(ExportProjectCharterReport).Name}-{row.RiskMatrix.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardStartById).Name}-{row.RiskMatrix.ProjectId}";
                    var cacheKeyAll = $"{typeof(GetAllRiskResponseActionsByRiskId).Name}{row.RiskMatrixId}";
                    _context.InvalidateCache(cacheKeyAll);

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(RiskResponseAction).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(RiskResponseAction).Name} was not deleted"
                });
            });

            // --- CHANGE ORDER ---
            app.MapPost("ChangeOrderRiskResponseAction", async (ChangeRiskResponseActionOrder dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.RiskResponseActions.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(RiskResponseAction).Name} was not found"
                    });
                }

                // Buscamos el item que ocupa el lugar al que queremos ir
                var NewRow = await _context.RiskResponseActions
                    .Where(x => x.RiskMatrixId == dto.RiskMatrixId && x.Order == dto.NewOrder && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (NewRow == null)
                {
                    // Si no hay nadie en ese puesto, simplemente movemos (o es un error de lógica, según tu caso)
                    // En tu ejemplo se retorna error si no se encuentra.
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(RiskResponseAction).Name} target position not found"
                    });
                }

                // Intercambio de posiciones (Swap)
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;

                if (await _context.SaveChangesAsync() > 0)
                {
                    var cacheKeyAll = $"{typeof(GetAllRiskResponseActionsByRiskId).Name}{dto.RiskMatrixId}";
                    _context.InvalidateCache(cacheKeyAll);

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(RiskResponseAction).Name} was reorder"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(RiskResponseAction).Name} was not reorder"
                });
            });
        }
    }
}
