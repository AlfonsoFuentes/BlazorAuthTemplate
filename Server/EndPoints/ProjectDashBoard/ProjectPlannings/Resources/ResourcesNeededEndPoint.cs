using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Projects.Plannings.Resources;

namespace Server.EndPoints.ProjectDashBoard.ProjectPlannings.ResourcesNeededs
{
    public class ResourcesNeededEndPoint : IEndPoint
    {
        void MapFromDto(ResourcesNeededDto dto, Resource row)
        {
            row.Name = dto.Name;
            row.Speciality = dto.Speciality;

        }
        static ResourcesNeededDto MapToDto(Resource row)
        {
            ResourcesNeededDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Speciality = row.Speciality;

            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("CreateResourcesNeeded", async (CreateResourcesNeeded dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Resource
                {

                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Resources.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var cacheKeyAll = $"{typeof(GetAllResourcesNeeded).Name}{dto.ProjectId}";
                var maxOrder = await getNextOrder.GetNextOrderAsync<Resource>(cacheKeyAll, dto.ProjectId);
                row.Order = maxOrder;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyExportProjectPlannPDF = $"{typeof(ExportProjectPlannPDF).Name}-{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardStartById).Name}-{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectPlannPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Resource).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Resource).Name} was not created successfully."
                });
            });
            app.MapPost("EditResourcesNeeded", async (EditResourcesNeeded dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = await _context.Resources.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Risk Matrix not found."
                    });
                MapFromDto(dto, row);


                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;



                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyAll = $"{typeof(GetAllResourcesNeeded).Name}{dto.ProjectId}";
                    var cacheKeyExportProjectPlannPDF = $"{typeof(ExportProjectPlannPDF).Name}-{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardStartById).Name}-{dto.ProjectId}";
                    var cacheKeyId = $"{typeof(GetResourcesNeededById).Name}-{dto.Id}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyId, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectPlannPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Resource).Name} Updated successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Resource).Name} was not Updated successfully."
                });
            });
            app.MapPost("GetResourcesNeededById", async (GetResourcesNeededById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetResourcesNeededById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Resources
                  
                     .Where(x => x.Id == request.Id)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync();

                }, cacheKey);

                if (row == null)
                    return Results.Ok(new GeneralDto<ResourcesNeededDto>
                    {
                        Succeeded = false,
                        Message = "Risk Matrix not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<ResourcesNeededDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });
            app.MapPost("GetAllResourcesNeeded", async (GetAllResourcesNeeded dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllResourcesNeeded).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Resources
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<ResourcesNeededDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteResourcesNeeded", async (DeleteResourcesNeeded dto, IAppDbContext _context) =>
            {
                var row = await _context.Resources.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Resource).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.Resources.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await _context.SaveChangesAsync();
                    var cacheKeyExportProjectPlannPDF = $"{typeof(ExportProjectPlannPDF).Name}-{dto.ProjectId}";
                    var cacheKeyAll = $"{typeof(GetAllResourcesNeeded).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardStartById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectPlannPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Resource).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Resource).Name} was not deleted"
                });
            });
            app.MapPost("ChangeOrderResourcesNeeded", async (ChangeOrderResourcesNeeded dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Resources.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Resource).Name} was not found"
                    });

                }

                var NewRow = await _context.Resources.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Resource).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllResourcesNeeded).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Resource).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Resource).Name} was not reorder"
                });
            });
        }
    }
}
