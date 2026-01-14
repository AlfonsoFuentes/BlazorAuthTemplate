using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Interfaces.EndPoints;
using Shared.Dtos.General;
using Shared.Dtos.Plannings.RiskMatrixs;
using Shared.Dtos.Projects;
using Shared.Enums.ProjectDefinitionTypes;
using System;
using System.Net.NetworkInformation;

namespace Server.EndPoints.ProjectDashBoard
{
    public class ProjectDashBoardEndPoint : IEndPoint
    {
        static ProjectDashboardDto MapToProjectDashboardDto(Project row)
        {
            ProjectDashboardDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Status = row.StatusEnum;
            dto.StartDate = row.StartDate;
            dto.LastModifiedOn = row.LastModifiedOn;
            dto.ProjectCode = string.IsNullOrEmpty(row.ProjectNumber) ? string.Empty : $"CEC0000{row.ProjectNumber}";
            dto.LastVisitedPhase = row.LastVisitedPhase;
            return dto;

        }
        static ProjectDashboardStartDto MapToStartDashboardDto(Project row)
        {
            ProjectDashboardStartDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Status = row.StatusEnum;
            dto.StartDate = row.StartDate;
            dto.LastModifiedOn = row.LastModifiedOn;
            dto.ProjectCode = string.IsNullOrEmpty(row.ProjectNumber) ? string.Empty : $"CEC0000{row.ProjectNumber}";


            // --- FINANCIALS ---
            var capital = row.BudgetItems.Where(x => x.IsNoExpenseTaxesEngCont).Sum(x => x.BudgetUSD);
            dto.ExpensesUSD = row.BudgetItems.Where(x => x.IsExpense).Sum(x => x.BudgetUSD);
            if (!row.IsProductiveAsset)
            {
                capital += capital * (decimal)row.PercentageTaxProductive / 100;
            }
            var engCont = (decimal)row.PercentageEngineering + (decimal)row.PercentageContingency;
            if ((100 - engCont) > 0)
            {
                capital = capital / (100 - engCont) * 100;
            }

            dto.CapitalUSD = capital;
            // --- PMI CHECKLIST (Calculado en SQL) ---

            // 1. Business Case: Debe tener al menos 1 Background o 1 Benefit
            dto.HasBusinessCase = row.DefinitionItems.Any(pd => pd.Type == ProjectDefinitionType.Background || pd.Type == ProjectDefinitionType.Benefit
            );

            // 2. Objectives: Al menos 1 Objetivo
            dto.HasObjectives = row.DefinitionItems.Any(pd => pd.Type == ProjectDefinitionType.Objective);

            // 3. Scope: Al menos 1 Deliverable o 1 Scope definition
            dto.HasScope = row.DefinitionItems.Any(pd => pd.Type == ProjectDefinitionType.Deliverable || pd.Type == ProjectDefinitionType.Scope);

            dto.HasRequirements = row.Requirements.Any();
            // 4. Stakeholders: Al menos 1 persona interesada
            dto.HasStakeholders = row.StakeHolders.Any();

            // 5. Risks: Al menos 1 riesgo identificado (no importa el nivel)
            dto.HasRisks = row.RiskMatrixs.Any();

            // --- COUNTERS (Para visualización) ---
            dto.StakeholderCount = row.StakeHolders.Count();
            dto.HighRiskCount = row.RiskMatrixs.Count(r => r.Impact == RiskImpact.Major || r.Impact == RiskImpact.Critical);
            dto.ObjectivesCount = row.DefinitionItems.Count(pd => pd.Type == ProjectDefinitionType.Objective);
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("GetAllProjectDashBoards", async (GetAllProjectDashBoards dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllProjectDashBoards).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToProjectDashboardDto).ToList();

                return Results.Ok(new GeneralDto<List<ProjectDashboardDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("GetProjectDashBoardById", async (GetProjectDashBoardById request, IAppDbContext _context) =>
            {
                // Clave de caché específica para este dashboard
                var cacheKey = $"{typeof(GetProjectDashBoardById).Name}-{request.Id}";

                var project = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects

                        .AsNoTracking()
                        .AsSplitQuery()
                        .AsQueryable()
                        .Where(x => x.Id == request.Id)
                        .FirstOrDefaultAsync();

                }, cacheKey);

                if (project is null)
                {
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Project not found" });
                }
                var dto = MapToProjectDashboardDto(project);
                return Results.Ok(new GeneralDto<ProjectDashboardDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });
            app.MapPost("GetProjectDashBoardStartById", async (GetProjectDashBoardStartById request, IAppDbContext _context) =>
            {
                // Clave de caché específica para este dashboard
                var cacheKey = $"{typeof(GetProjectDashBoardStartById).Name}-{request.Id}";

                var project = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                    .Include(x => x.BudgetItems)
                    .Include(x => x.DefinitionItems)
                    .Include(x => x.StakeHolders)
                    .Include(x => x.RiskMatrixs)
                    .Include(x => x.Requirements)
                        .AsNoTracking()
                        .AsSplitQuery()
                        .AsQueryable()
                        .Where(x => x.Id == request.Id)
                        .FirstOrDefaultAsync();

                }, cacheKey);

                if (project is null)
                {
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Project not found" });
                }
                var dto = MapToStartDashboardDto(project);
                return Results.Ok(new GeneralDto<ProjectDashboardStartDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });
            app.MapPost("UpdateProjectLastPhaseDto", async (UpdateProjectLastPhaseDto request, IAppDbContext _context) =>
            {


                // Opción B: Estándar (Compatible con todo)
                var project = await _context.Projects.FindAsync(request.ProjectId);
                if (project != null)
                {
                    project.LastVisitedPhase = request.PhaseId;
                    await _context.SaveChangesAsync();
                }

                return Results.Ok(new GeneralDto { Succeeded = true });
            });



        }
    }
}
