using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Instruments;
using Server.Interfaces.EndPoints;
using Server.Services;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.Instruments;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Instruments
{

    public class InstrumentEndPoints : IEndPoint
    {

        void MapFromDto(InstrumentDto dto, Instrument row)
        {
            row.Name = dto.Name;


            row.BudgetUSD = dto.BudgetUSD;

        }
        public static InstrumentDto MapToDto(Instrument row)
        {
            InstrumentDto dto = new();
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
            app.MapPost("CreateInstrument", async (CreateInstrument dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Instrument
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Instruments.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;
                var cacheKeyAll = $"{typeof(GetAllInstruments).Name}{dto.ProjectId}";
                var maxOrder = await getNextOrder.GetNextOrderAsync<Instrument>(cacheKeyAll, dto.ProjectId);
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
                        Message = $"{typeof(Instrument).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(Instrument).Name} was not created successfully."
                });


            });

         

            // ✅ Editar
            app.MapPost("EditInstrument", async (EditInstrument dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Instruments.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Instrument).Name} not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);
                    var cacheKeyId = $"{typeof(GetInstrumentById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllInstruments).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Instrument).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Instrument).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetInstrumentById", async (GetInstrumentById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetInstrumentById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Instruments

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<InstrumentDto>
                    {
                        Succeeded = false,
                        Message = $"{typeof(Instrument).Name} not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<InstrumentDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllInstruments", async (GetAllInstruments dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllInstruments).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Instruments
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<InstrumentDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteInstrument", async (DeleteInstrument dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Instruments.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Instrument).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.Instruments.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);

                    var cacheKeyAll = $"{typeof(GetAllInstruments).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Instrument).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Instrument).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateInstrumentName", async (ValidateInstrumentName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllInstruments).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Instruments
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<Instrument, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderInstrument", async (ChangeOrderInstrument dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Instruments.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Instrument).Name} was not found"
                    });

                }

                var NewRow = await _context.Instruments.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Instrument).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllInstruments).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Instrument).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Instrument).Name} was not reorder"
                });
            });


        }
    }
}

