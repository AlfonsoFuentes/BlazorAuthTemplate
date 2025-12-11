using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Domain.CommonEntities.BudgetItems.Commons;
using Server.Domain.CommonEntities.BudgetItems.EngineeringContingency;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Equipments;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Instruments;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Pipings;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Valves;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Alterations;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Contingencys;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.EHSs;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Electricals;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.EngineeringDesigns;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Engineerings;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Equipments;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Foundations;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Instruments;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Paintings;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Pipes;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Structurals;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Taxs;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Testings;
using Server.EndPoints.ProjectDashBoard.ProjectStarts.Valves;
using Server.Interfaces.EndPoints;
using Server.Services;
using Shared.Dtos.General;
using Shared.Dtos.Starts.Alterations;
using Shared.Dtos.Starts.Investments;
using Shared.Dtos.Starts.Pipes;
using System.Diagnostics;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Investments.Queries
{
    public class InvestmentEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("GetInvestmentById", async (GetInvestmentById request, IAppDbContext _context) =>
            {
                var cacheKeyGetInvestmentById = $"{typeof(GetInvestmentById).Name}-{request.Id}"; //TODO: cuando se pueda tiparlo mejor
                var project = await _context.GetOrAddCacheAsync(async () =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    var result = await _context.Projects
                    .Include(x => x.BudgetItems)
                   .Where(x => x.Id == request.Id)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync();

                    stopwatch.Stop();
                    Console.WriteLine($"[Cache MISS] GetInvestmentById {stopwatch.ElapsedMilliseconds} ms");
                    return result;
                }, cacheKeyGetInvestmentById);
                if (project == null)
                {
                    return Results.Ok(new GeneralDto<InvestmentDto>
                    {
                        Succeeded = false,
                        Message = $"{typeof(Project).Name} not found."
                    });
                }
                InvestmentDto result = new InvestmentDto()
                {
                    Alterations = project.BudgetItems.OfType<Alteration>().Select(AlterationEndPoints.MapToDto).ToList(),
                    Foundations = project.BudgetItems.OfType<Foundation>().Select(FoundationEndPoints.MapToDto).ToList(),
                    Structurals = project.BudgetItems.OfType<Structural>().Select(StructuralEndPoints.MapToDto).ToList(),
                    Equipments = project.BudgetItems.OfType<Equipment>().Select(EquipmentEndPoints.MapToDto).ToList(),
                    Valves = project.BudgetItems.OfType<Valve>().Select(ValveEndPoints.MapToDto).ToList(),
                    Electricals = project.BudgetItems.OfType<Electrical>().Select(ElectricalEndPoints.MapToDto).ToList(),
                    Pipes = project.BudgetItems.OfType<Pipe>().Select(PipeEndPoints.MapToDto).ToList(),
                    Instruments = project.BudgetItems.OfType<Instrument>().Select(InstrumentEndPoints.MapToDto).ToList(),
                    Paintings = project.BudgetItems.OfType<Painting>().Select(PaintingEndPoints.MapToDto).ToList(),
                    EHSs = project.BudgetItems.OfType<EHS>().Select(EHSEndPoints.MapToDto).ToList(),
                    Taxes = project.BudgetItems.OfType<Tax>().Select(TaxEndPoints.MapToDto).ToList(),
                    Testings = project.BudgetItems.OfType<Testing>().Select(TestingEndPoints.MapToDto).ToList(),
                    EngineeringDesigns = project.BudgetItems.OfType<EngineeringDesign>().Select(EngineeringDesignPoints.MapToDto).ToList(),
                    EngineeringSalarys = project.BudgetItems.OfType<Engineering>().Select(EngineeringSalaryEndPoints.MapToDto).ToList(),
                    Contingencies = project.BudgetItems.OfType<Contingency>().Select(ContingencyEndPoints.MapToDto).ToList(),
                };
                return Results.Ok(new GeneralDto<InvestmentDto>
                {
                    Succeeded = true,
                    Data = result
                });
            });
        }
    }
}
