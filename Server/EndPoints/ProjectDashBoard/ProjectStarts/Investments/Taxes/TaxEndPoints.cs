using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.Contingencys;
using Shared.Dtos.Starts.Engineerings;
using Shared.Dtos.Starts.Foundations;
using Shared.Dtos.Starts.Taxs;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Taxs
{

    public class TaxEndPoints : IEndPoint
    {

        void MapFromDto(TaxDto dto, Tax row)
        {
            row.Name = dto.Name;
            row.Percentage = dto.Percentage;


        }
        public static TaxDto MapToDto(Tax row)
        {
            TaxDto dto = new();
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
            app.MapPost("EditTax", async (EditTax dto, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Taxes.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Tax).Name} not found."
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
                        Message = $"{typeof(Tax).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Tax).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetTaxById", async (GetTaxById request, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var cacheKey = $"{typeof(GetTaxById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Taxes

                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);
                if (row == null)
                    return Results.Ok(new GeneralDto<TaxDto>
                    {
                        Succeeded = false,
                        Message = $"{typeof(Tax).Name} not found."
                    });

                var dto = MapToDto(row);
                dto.CapitalBudget = await engCalculation.GetCapitalBudgetForTaxes(dto.ProjectId);
               
                return Results.Ok(new GeneralDto<TaxDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });
           
            
            // ✅ Obtener todos
            app.MapPost("GetAllTaxs", async (GetAllTaxs dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllTaxs).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Taxes
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<TaxDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });



        }
    }
}

