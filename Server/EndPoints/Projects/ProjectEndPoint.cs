using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Domain.CommonEntities.BudgetItems.EngineeringContingency;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Enums.CostCenter;
using Shared.Enums.Focuses;
using Shared.Enums.ProjectNeedTypes;

namespace Server.EndPoints.Projects
{
    public class ProjectEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("ValidateProjectName", async (ValidateProjectName dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllProjectDashBoards).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                Func<Project, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ValidateProjectNumber", async (ValidateProjectNumber dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllProjectDashBoards).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                Func<Project, bool> predicate = x => dto.Id == Guid.Empty ? x.ProjectNumber.Equals(dto.ProjectNumber) : x.Id != dto.Id && x.ProjectNumber.Equals(dto.ProjectNumber);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "ProjectNumber is available." : "ProjectNumber already in use."
                };
            });
            app.MapPost("GetProjectToApproveById", async (GetProjectToApproveById dto, IAppDbContext _context) =>
            {
                var project = await _context.Projects
                 .Include(x => x.BackGrounds)
                 .Include(x => x.StakeHolders)
                 .Include(x => x.Requirements)
                 .Include(x => x.Objectives)
                 .Include(x => x.Scopes)
                 .Include(x => x.AcceptanceCriterias)
                 .Include(x => x.Bennefits)
                 .Include(x => x.BudgetItems)

                .Where(x => x.Id == dto.Id)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().FirstOrDefaultAsync();

                if (project == null)
                {
                    return Results.Ok(new GeneralDto<ApproveProject>
                    {
                        Succeeded = false,
                        Data = null!,
                        Message = "Project not found."
                    });
                }
                var totalBudgetItems = project.BudgetItems.Count;
                var engineering = project.BudgetItems.OfType<Engineering>().OrderBy(x => x.Order).FirstOrDefault();
                if (engineering != null) totalBudgetItems--;

                var contingency = project.Contingencys.OfType<Contingency>().OrderBy(x => x.Order).FirstOrDefault();
                if (contingency != null) totalBudgetItems--;

                var taxes = project.Taxes.OrderBy(x => x.Order).FirstOrDefault();
                if (taxes != null) totalBudgetItems--;

                var response = new ApproveProject()
                {
                    Id = dto.Id,
                    PercentageEngineering = engineering?.Percentage ?? 0,
                    PercentageContingency = contingency?.Percentage ?? 0,
                    PercentageTaxProductive = project.PercentageTaxProductive,
                    IsProductiveAsset = project.IsProductiveAsset,
                    CostCenter = CostCenterEnum.GetType(project.CostCenter),
                    Focus = FocusEnum.GetType(project.Focus),
                    ProjectNeedType = ProjectNeedTypeEnum.GetType(project.ProjectNeedType),
                    ProjectName = project.Name,
                    ProjectNumber = project.ProjectNumber,
                    InitialProjectDate = project.StartDate,
                    BudgetItems = totalBudgetItems,
                    Stakeholders = project.StakeHolders.Count,
                    Requirements = project.Requirements.Count,
                    Objectives = project.Objectives.Count,
                    Scopes = project.Scopes.Count,
                    AcceptenceCriterias = project.AcceptanceCriterias.Count,
                    Backgrounds = project.BackGrounds.Count



                };

                return Results.Ok(new GeneralDto<ApproveProject>
                {
                    Succeeded = true,
                    Data = response,
                    Message = "Project retrieved successfully."
                });

            });
            app.MapPost("CreateProject", async (CreateProject request, IAppDbContext _context) =>
            {
                var row = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = request.ProjectName,
                    Status = ProjectStatusEnum.Created.Id,
                };
                await _context.Projects.AddAsync(row);
                var contingency = Contingency.Create(row.Id);
                contingency.Order = 1;

                var engineering = Engineering.Create(row.Id);
                engineering.Order = 1;

                engineering.Percentage = request.PercentageEngineering;
                contingency.Percentage = request.PercentageContingency;

                engineering.Name = $"Engineering {request.PercentageEngineering}%";
                contingency.Name = $"Contingency {request.PercentageContingency}%";

                await _context.EngineeringSalarys.AddAsync(engineering);
                await _context.Contingencys.AddAsync(contingency);
                row.IsProductiveAsset = request.IsProductiveAsset;
                if (!request.IsProductiveAsset)
                {
                    var tax = new Tax
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = row.Id,
                        Name = $"Tax {request.PercentageTaxProductive}%",
                        Percentage = request.PercentageTaxProductive,
                        Order = 1,
                    };
                    await _context.Taxes.AddAsync(tax);
                }

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKey = $"{typeof(GetAllProjectDashBoards).Name}";
                    _context.InvalidateCache(cacheKey);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Project).Name} created successfully"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Project).Name} was not created successfully"
                });
            });
            app.MapPost("ApproveProject", async (ApproveProject request, IAppDbContext _context, IRepositoryInvestmentCalculation engCalculation) =>
            {
                var row = await _context.Projects.FindAsync(request.Id);
                if (row == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Project).Name} was not found"
                    });
                }
                row.ProjectNumber = request.ProjectNumber;
                row.Status = ProjectStatusEnum.PLANNING_ID;
                row.StartDate = request.InitialProjectDate;
                var contingency = await _context.Contingencys.FirstOrDefaultAsync(x => x.ProjectId == row.Id);


                var engineering = await _context.EngineeringSalarys.FirstOrDefaultAsync(x => x.ProjectId == row.Id);
                if (contingency != null && engineering != null)
                {
                    engineering?.Percentage = request.PercentageEngineering;
                    contingency?.Percentage = request.PercentageContingency;
                }


                if (!row.IsProductiveAsset)
                {
                    var tax = await _context.Taxes.FirstOrDefaultAsync(x => x.ProjectId == row.Id);
                    tax!.Percentage = request.PercentageTaxProductive;
                }
                else if (request.IsProductiveAsset)
                {
                    var existingTax = await _context.Taxes.FirstOrDefaultAsync(x => x.ProjectId == row.Id);
                    if (existingTax != null)
                    {
                        _context.Taxes.Remove(existingTax);
                    }
                    row.IsProductiveAsset = true;
                }



                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await engCalculation.CalculateEngineeringTotalCost(row.Id);
                    var cacheKeyProjectDashBoardById = $"{typeof(GetProjectDashBoardById).Name}-{row.Id}";
                    var cacheKeyAllProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    _context.InvalidateCache(cacheKeyAllProjectDashBoards, cacheKeyProjectDashBoardById);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Project).Name} created successfully"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Project).Name} was not created successfully"
                });
            });
        }
    }
}
