using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.AcceptanceCriterias;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.AcceptanceCriterias
{

    public class AcceptanceCriteriaEndPoints : IEndPoint
    {

        void MapFromDto(AcceptanceCriteriaDto dto, AcceptanceCriteria row)
        {
            row.Name = dto.Name;

        }
        static AcceptanceCriteriaDto MapToDto(AcceptanceCriteria row)
        {
            AcceptanceCriteriaDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateAcceptanceCriteria", async (CreateAcceptanceCriteria dto, IAppDbContext _context, ICache _cache) =>
            {
                var row = new AcceptanceCriteria
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.AcceptanceCriterias.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var cacheKeyAll = $"{typeof(GetAllAcceptanceCriterias).Name}{dto.ProjectId}";
                var rows = await _cache.GetOrAddCacheAsync(async () =>
                {
                    return await _context.AcceptanceCriterias
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKeyAll);

                var maxOrder = rows!.Max(x => x.Order) + 1;
                row.Order = maxOrder;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {

                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeycacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";

                    _cache.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeycacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = true,
                        Message = $"{typeof(AcceptanceCriteria).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Suceeded = true,
                    Message = $"{typeof(AcceptanceCriteria).Name} was not created successfully."
                });


            });


            // ✅ Editar
            app.MapPost("EditAcceptanceCriteria", async (EditAcceptanceCriteria dto, IAppDbContext _context, ICache _cache) =>
            {
                var row = await _context.AcceptanceCriterias.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = false,
                        Message = "Acceptance Criteria not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyId = $"{typeof(GetAcceptanceCriteriaById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllAcceptanceCriterias).Name}{dto.ProjectId}";
                    var cacheKeycacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _cache.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeycacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = true,
                        Message = $"{typeof(AcceptanceCriteria).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Suceeded = false,
                    Message = $"{typeof(AcceptanceCriteria).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetAcceptanceCriteriaById", async (GetAcceptanceCriteriaById request, IAppDbContext _context, ICache _cache) =>
            {
                var row = await _context.AcceptanceCriterias.FindAsync(request.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto<AcceptanceCriteriaDto>
                    {
                        Suceeded = false,
                        Message = "Acceptance Criteria not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<AcceptanceCriteriaDto>
                {
                    Suceeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllAcceptanceCriterias", async (GetAllAcceptanceCriterias dto, IAppDbContext _context, ICache _cache) =>
            {
                var cacheKey = $"{typeof(GetAllAcceptanceCriterias).Name}{dto.ProjectId}";
                var rows = await _cache.GetOrAddCacheAsync(async () =>
                {
                    return await _context.AcceptanceCriterias
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<AcceptanceCriteriaDto>>
                {
                    Suceeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteAcceptanceCriteria", async (DeleteAcceptanceCriteria dto, IAppDbContext _context, ICache cache) =>
            {
                var row = await _context.AcceptanceCriterias.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = false,
                        Message = $"{typeof(AcceptanceCriteria).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.AcceptanceCriterias.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await _context.SaveChangesAsync();

                    var cacheKeyAll = $"{typeof(GetAllAcceptanceCriterias).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeycacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    cache.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeycacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = true,
                        Message = $"{typeof(AcceptanceCriteria).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Suceeded = false,
                    Message = $"{typeof(AcceptanceCriteria).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateAcceptanceCriteriaName", async (ValidateAcceptanceCriteriaName dto, IAppDbContext _context, ICache _cache) =>
            {
                var cacheKeyAll = $"{typeof(GetAllAcceptanceCriterias).Name}{dto.ProjectId}";
                var rows = await _cache.GetOrAddCacheAsync(async () =>
                {
                    return await _context.AcceptanceCriterias
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<AcceptanceCriteria, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Suceeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderAcceptanceCriteria", async (ChangeOrderAcceptanceCriteria dto, IAppDbContext _context, ICache _cache) =>
            {
                var CurrentRow = await _context.AcceptanceCriterias.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = false,
                        Message = $"{typeof(AcceptanceCriteria).Name} was not found"
                    });

                }

                var NewRow = await _context.AcceptanceCriterias.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = false,
                        Message = $"{typeof(AcceptanceCriteria).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllAcceptanceCriterias).Name}{dto.ProjectId}";

                    _cache.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = true,
                        Message = $"{typeof(AcceptanceCriteria).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Suceeded = false,
                    Message = $"{typeof(AcceptanceCriteria).Name} was not reorder"
                });
            });


        }
    }
}

