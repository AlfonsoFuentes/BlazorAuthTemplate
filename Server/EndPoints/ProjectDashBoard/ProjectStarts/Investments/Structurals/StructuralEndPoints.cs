using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems.Commons;
using Server.Interfaces.EndPoints;
using Server.Services;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.Structurals;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Structurals
{

    public class StructuralEndPoints : IEndPoint
    {

        void MapFromDto(StructuralDto dto, Structural row)
        {
            row.Name = dto.Name;
            row.UnitaryCost = dto.UnitaryCost;
            row.Quantity = dto.Quantity;


            row.BudgetUSD = dto.BudgetUSD;

        }
        public static StructuralDto MapToDto(Structural row)
        {
            StructuralDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            dto.UnitaryCost = row.UnitaryCost;
            dto.Quantity = row.Quantity;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateStructural", async (CreateStructural dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Structural
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Structurals.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;
                var cacheKeyAll = $"{typeof(GetAllStructurals).Name}{dto.ProjectId}";
                var maxOrder = await getNextOrder.GetNextOrderAsync<Structural>(cacheKeyAll, dto.ProjectId);
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
                        Message = $"{typeof(Structural).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Structural).Name} was not created successfully."
                });


            });

           
            
            // ✅ Editar
            app.MapPost("EditStructural", async (EditStructural dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Structurals.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Structural).Name} not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);
                    var cacheKeyId = $"{typeof(GetStructuralById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllStructurals).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Structural).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Structural).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetStructuralById", async (GetStructuralById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetStructuralById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Structurals

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<StructuralDto>
                    {
                        Succeeded = false,
                        Message = $"{typeof(Structural).Name} not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<StructuralDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllStructurals", async (GetAllStructurals dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllStructurals).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Structurals
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<StructuralDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteStructural", async (DeleteStructural dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Structurals.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Structural).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.Structurals.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);

                    var cacheKeyAll = $"{typeof(GetAllStructurals).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Structural).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Structural).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateStructuralName", async (ValidateStructuralName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllStructurals).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Structurals
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<Structural, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderStructural", async (ChangeOrderStructural dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Structurals.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Structural).Name} was not found"
                    });

                }

                var NewRow = await _context.Structurals.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Structural).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllStructurals).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Structural).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Structural).Name} was not reorder"
                });
            });


        }
    }
}

