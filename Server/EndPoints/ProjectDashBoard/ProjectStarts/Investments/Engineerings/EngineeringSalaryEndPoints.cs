using Azure.Core;
using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems.EngineeringContingency;
using Server.Interfaces.EndPoints;
using Server.Services;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.Contingencys;
using Shared.Dtos.Starts.Engineerings;
using Shared.Dtos.Starts.Foundations;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Engineerings
{

    public class EngineeringSalaryEndPoints : IEndPoint
    {

        void MapFromDto(EngineeringSalarysDto dto, Engineering row)
        {
            row.Name = dto.Name;
            row.Percentage = dto.Percentage;


        }
        public static EngineeringSalarysDto MapToDto(Engineering row)
        {
            EngineeringSalarysDto dto = new();
            dto.Id = row.Id;
           
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            dto.Percentage = row.Percentage;
            dto.BudgetUSD = row.BudgetUSD;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear




            // ✅ Editar
            app.MapPost("EditEngineeringSalarys", async (EditEngineeringSalarys dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.EngineeringSalarys.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Engineering).Name} not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await engCalculation.CalculateEngineeringTotalCost(dto.ProjectId);

                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Engineering).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Engineering).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetEngineeringSalaryById", async (GetEngineeringSalaryById request, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var cacheKey = $"{typeof(GetEngineeringSalaryById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.EngineeringSalarys

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<EngineeringSalarysDto>
                    {
                        Succeeded = false,
                        Message = $"{typeof(Engineering).Name} not found."
                    });

                var dto = MapToDto(row);
                dto.CapitalBudget = await engCalculation.GetCapitalBudget(dto.ProjectId);
                dto.ContingencyPercentage = row.Percentage;
                return Results.Ok(new GeneralDto<EngineeringSalarysDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });
           

            // ✅ Obtener todos
            app.MapPost("GetAllEngineeringSalarys", async (GetAllEngineeringSalarys dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllEngineeringSalarys).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.EngineeringSalarys
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<EngineeringSalarysDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });



        }
    }
}

