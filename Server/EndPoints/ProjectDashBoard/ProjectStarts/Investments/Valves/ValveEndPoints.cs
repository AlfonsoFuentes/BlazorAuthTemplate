using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems.Commons;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Valves;
using Server.Interfaces.EndPoints;
using Server.Services;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.Valves;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Valves
{

    public class ValveEndPoints : IEndPoint
    {

        void MapFromDto(ValveDto dto, Valve row)
        {
            row.Name = dto.Name;



            row.BudgetUSD = dto.BudgetUSD;

        }
        public static ValveDto MapToDto(Valve row)
        {
            ValveDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            dto.BudgetUSD = row.BudgetUSD;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateValve", async (CreateValve dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Valve
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Valves.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var cacheKeyAll = $"{typeof(GetAllValves).Name}{dto.ProjectId}";
                var maxOrder = await getNextOrder.GetNextOrderAsync<Valve>(cacheKeyAll, dto.ProjectId);
                row.Order = maxOrder;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);
                   
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Valve).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Valve).Name} was not created successfully."
                });


            });

            

            // ✅ Editar
            app.MapPost("EditValve", async (EditValve dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Valves.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Valve).Name} not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);
                    var cacheKeyId = $"{typeof(GetValveById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllValves).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Valve).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Valve).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetValveById", async (GetValveById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetValveById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Valves

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<ValveDto>
                    {
                        Succeeded = false,
                        Message = $"{typeof(Valve).Name} not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<ValveDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllValves", async (GetAllValves dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllValves).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Valves
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<ValveDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteValve", async (DeleteValve dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Valves.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Valve).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.Valves.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);

                    var cacheKeyAll = $"{typeof(GetAllValves).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Valve).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Valve).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateValveName", async (ValidateValveName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllValves).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Valves
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<Valve, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderValve", async (ChangeOrderValve dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Valves.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Valve).Name} was not found"
                    });

                }

                var NewRow = await _context.Valves.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Valve).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllValves).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Valve).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Valve).Name} was not reorder"
                });
            });


        }
    }
}

