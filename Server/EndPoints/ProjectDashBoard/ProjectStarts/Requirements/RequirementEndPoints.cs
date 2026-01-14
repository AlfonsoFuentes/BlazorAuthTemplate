using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.Requirements;
using Shared.Enums.DashBoardTable;
using Shared.Enums.RequirementPrioritys;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Requirements
{

    public class RequirementEndPoints : IEndPoint
    {

        void MapFromDto(RequirementDto dto, Requirement row)
        {
            row.Name = dto.Name;
            row.Order = dto.Order;
            row.ProjectId = dto.ProjectId;
            row.ResponsibleId = dto.Responsible?.Id;
            row.Type = dto.Type.Name;
            row.RequestedById = dto.RequestedBy?.Id;
            row.Priority = dto.Priority.Name;

        }
        static RequirementDto MapToDto(Requirement row)
        {
            RequirementDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            dto.Type = row.TypeEnum;
            dto.RequestedBy = row.RequestedById.HasValue ? new StakeHolderDto
            {
                Id = row.RequestedBy!.Id,
                Name = row.RequestedBy.Name
            } : null;
            dto.Priority = row.PriorityEnum;
            dto.Responsible = row.ResponsibleId.HasValue ? new StakeHolderDto
            {
                Id = row.Responsible!.Id,
                Name = row.Responsible.Name
            } : null;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateRequirement", async (CreateRequirement dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Requirement
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Requirements.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;



                var maxOrder = await _context.Requirements
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .MaxAsync(x => (int?)x.Order) ?? 0;

                row.Order = maxOrder + 1;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Requirements);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Requirement).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Requirement).Name} was not created successfully."
                });


            });


            // ✅ Editar
            app.MapPost("EditRequirement", async (EditRequirement dto, IAppDbContext _context) =>
            {
                var row = await _context.Requirements.FindAsync(dto.Id);
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
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Requirements);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Requirement).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Requirement).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetRequirementById", async (GetRequirementById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetRequirementById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Requirements
                    .Include(x => x.RequestedBy)
                    .Include(x => x.Responsible)

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<RequirementDto>
                    {
                        Succeeded = false,
                        Message = "Requirement not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<RequirementDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllRequirements", async (GetAllRequirements dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllRequirements).Name}-{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Requirements
                    .Include(x => x.RequestedBy)
                    .Include(x => x.Responsible)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<RequirementDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteRequirement", async (DeleteRequirement dto, IAppDbContext _context) =>
            {
                var row = await _context.Requirements.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Requirement).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.Requirements.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await _context.SaveChangesAsync();
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Requirements);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Requirement).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Requirement).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateRequirementName", async (ValidateRequirementName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllRequirements).Name}-{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Requirements.Where(x => x.ProjectId == dto.ProjectId)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<Requirement, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderRequirement", async (ChangeOrderRequirement dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Requirements.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Requirement).Name} was not found"
                    });

                }

                var NewRow = await _context.Requirements.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Requirement).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllRequirements).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Requirement).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Requirement).Name} was not reorder"
                });
            });


        }
    }
}

