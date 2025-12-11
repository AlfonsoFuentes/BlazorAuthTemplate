using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.Requirements;
using Shared.Dtos.Starts.Scopes;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Scopes
{

    public class ScopeEndPoints : IEndPoint
    {

        void MapFromDto(ScopeDto dto, Scope row)
        {
            row.Name = dto.Name;

        }
        static ScopeDto MapToDto(Scope row)
        {
            ScopeDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateScope", async (CreateScope dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Scope
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Scopes.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var cacheKeyAll = $"{typeof(GetAllScopes).Name}{dto.ProjectId}";
              
                var maxOrder = await getNextOrder.GetNextOrderAsync<Scope>(cacheKeyAll, dto.ProjectId);
                row.Order = maxOrder;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Scope).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Scope).Name} was not created successfully."
                });


            });


            // ✅ Editar
            app.MapPost("EditScope", async (EditScope dto, IAppDbContext _context) =>
            {
                var row = await _context.Scopes.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Acceptance Criteria not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyId = $"{typeof(GetScopeById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllScopes).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Scope).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Scope).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetScopeById", async (GetScopeById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetScopeById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Scopes

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<ScopeDto>
                    {
                        Succeeded = false,
                        Message = "Scope not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<ScopeDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllScopes", async (GetAllScopes dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllScopes).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Scopes
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<ScopeDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteScope", async (DeleteScope dto, IAppDbContext _context) =>
            {
                var row = await _context.Scopes.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Scope).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.Scopes.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await _context.SaveChangesAsync();
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyAll = $"{typeof(GetAllScopes).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById,cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Scope).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Scope).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateScopeName", async (ValidateScopeName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllScopes).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Scopes
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<Scope, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderScope", async (ChangeOrderScope dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Scopes.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Scope).Name} was not found"
                    });

                }

                var NewRow = await _context.Scopes.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Scope).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllScopes).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Scope).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Scope).Name} was not reorder"
                });
            });


        }
    }
}

