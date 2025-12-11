using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems.Commons;
using Server.Interfaces.EndPoints;
using Server.Services;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.BackGrounds;
using Shared.Dtos.Starts.Contingencys;
using Shared.Dtos.Starts.Testings;
using Shared.Dtos.Starts.Taxs;
using Shared.Enums.CostCenter;
using Server.Services.Repositories;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Testings
{

    public class TestingEndPoints : IEndPoint
    {

        void MapFromDto(TestingDto dto, Testing row)
        {
            row.Name = dto.Name;



            row.BudgetUSD = dto.BudgetUSD;

        }
        public static TestingDto MapToDto(Testing row)
        {
            TestingDto dto = new();
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
            app.MapPost("CreateTesting", async (CreateTesting dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Testing
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Testings.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var cacheKeyAll = $"{typeof(GetAllTestings).Name}{dto.ProjectId}";
                var maxOrder = await getNextOrder.GetNextOrderAsync<Testing>(cacheKeyAll, dto.ProjectId);
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
                        Message = $"{typeof(Testing).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Testing).Name} was not created successfully."
                });


            });

           
            // ✅ Editar
            app.MapPost("EditTesting", async (EditTesting dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Testings.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Testing).Name} not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);
                    var cacheKeyId = $"{typeof(GetTestingById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllTestings).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Testing).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Testing).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetTestingById", async (GetTestingById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetTestingById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Testings

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<TestingDto>
                    {
                        Succeeded = false,
                        Message = $"{typeof(Testing).Name} not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<TestingDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllTestings", async (GetAllTestings dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllTestings).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Testings
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<TestingDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteTesting", async (DeleteTesting dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Testings.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Testing).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.Testings.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);

                    var cacheKeyAll = $"{typeof(GetAllTestings).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Testing).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Testing).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateTestingName", async (ValidateTestingName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllTestings).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Testings
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<Testing, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderTesting", async (ChangeOrderTesting dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Testings.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Testing).Name} was not found"
                    });

                }

                var NewRow = await _context.Testings.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Testing).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllTestings).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Testing).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Testing).Name} was not reorder"
                });
            });


        }
    }
}

