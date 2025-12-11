using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems.Commons;
using Server.Interfaces.EndPoints;
using Server.Services;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.Assumptions;
using Shared.Dtos.Starts.BackGrounds;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.BackGrounds
{

    public class BackGroundEndPoints : IEndPoint
    {

        void MapFromDto(BackGroundDto dto, BackGround row)
        {
            row.Name = dto.Name;

        }
        static BackGroundDto MapToDto(BackGround row)
        {
            BackGroundDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateBackGround", async (CreateBackGround dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new BackGround
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.BackGrounds.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var cacheKeyAll = $"{typeof(GetAllBackGrounds).Name}{dto.ProjectId}";
                

                var maxOrder = await getNextOrder.GetNextOrderAsync<BackGround>(cacheKeyAll, dto.ProjectId);
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
                        Message = $"{typeof(BackGround).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(BackGround).Name} was not created successfully."
                });


            });


            // ✅ Editar
            app.MapPost("EditBackGround", async (EditBackGround dto, IAppDbContext _context) =>
            {
                var row = await _context.BackGrounds.FindAsync(dto.Id);
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
                    var cacheKeyId = $"{typeof(GetBackGroundById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllBackGrounds).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(BackGround).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(BackGround).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetBackGroundById", async (GetBackGroundById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetBackGroundById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.BackGrounds

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<BackGroundDto>
                    {
                        Succeeded = false,
                        Message = "Background not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<BackGroundDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllBackGrounds", async (GetAllBackGrounds dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllBackGrounds).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.BackGrounds
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<BackGroundDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteBackGround", async (DeleteBackGround dto, IAppDbContext _context) =>
            {
                var row = await _context.BackGrounds.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(BackGround).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.BackGrounds.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await _context.SaveChangesAsync();
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyAll = $"{typeof(GetAllBackGrounds).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(BackGround).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(BackGround).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateBackGroundName", async (ValidateBackGroundName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllBackGrounds).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.BackGrounds
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<BackGround, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderBackGround", async (ChangeOrderBackGround dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.BackGrounds.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(BackGround).Name} was not found"
                    });

                }

                var NewRow = await _context.BackGrounds.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(BackGround).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllBackGrounds).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(BackGround).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(BackGround).Name} was not reorder"
                });
            });


        }
    }
}

