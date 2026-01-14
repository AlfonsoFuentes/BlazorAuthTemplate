using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Interfaces.EndPoints;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
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
                 //.Include(x => x.BackGrounds)
                 .Include(x => x.StakeHolders)
                 .Include(x => x.Requirements)
                 //.Include(x => x.Objectives)
                 //.Include(x => x.Scopes)
                 //.Include(x => x.AcceptanceCriterias)
                 //.Include(x => x.Bennefits)
                 .Include(x => x.BudgetItems)

                .Where(x => x.Id == dto.Id)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().FirstOrDefaultAsync();

                if (project == null)
                {
                    return Results.Ok(new GeneralDto<ApproveProjectStart>
                    {
                        Succeeded = false,
                        Data = null!,
                        Message = "Project not found."
                    });
                }
                //var totalBudgetItems = project.BudgetItems.Count;
                //var engineering = project.BudgetItems.OfType<Engineering>().OrderBy(x => x.Order).FirstOrDefault();
                //if (engineering != null) totalBudgetItems--;

                //var contingency = project.Contingencys.OfType<Contingency>().OrderBy(x => x.Order).FirstOrDefault();
                //if (contingency != null) totalBudgetItems--;

                //var taxes = project.Taxes.OrderBy(x => x.Order).FirstOrDefault();
                //if (taxes != null) totalBudgetItems--;

                var response = new ApproveProjectStart()
                {
                    Id = dto.Id,

                    ProjectName = project.Name,

                    //BudgetItems = totalBudgetItems,
                    Stakeholders = project.StakeHolders.Count,
                    Requirements = project.Requirements.Count,
                    //Objectives = project.Objectives.Count,
                    //Scopes = project.Scopes.Count,
                    //AcceptenceCriterias = project.AcceptanceCriterias.Count,
                    //Backgrounds = project.BackGrounds.Count



                };

                return Results.Ok(new GeneralDto<ApproveProjectStart>
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
               
                row.IsProductiveAsset = request.IsProductiveAsset;
                row.PercentageContingency=request.PercentageContingency;
                row.PercentageEngineering=request.PercentageEngineering;
                row.PercentageTaxProductive=request.PercentageTaxProductive;

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
            app.MapPost("ApproveProjectStart", async (ApproveProjectStart request, IAppDbContext _context) =>
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

                row.Status = ProjectStatusEnum.PLANNING_ID;
                row.StartDate = request.InitialProjectDate;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyProjectStartDateById = $"{typeof(GetInitialProjectDate).Name}-{row.Id}";
                    var cacheKeyProjectDashBoardById = $"{typeof(GetProjectDashBoardStartById).Name}-{row.Id}";
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
            app.MapPost("ApproveProjectPlann", async (ApproveProjectPlann request, IAppDbContext _context/*, IRepositoryInvestmentCalculation engCalculation*/) =>
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
                row.Status = ProjectStatusEnum.EXECUTION_ID;

                //var contingency = await _context.Contingencys.FirstOrDefaultAsync(x => x.ProjectId == row.Id);


                //var engineering = await _context.EngineeringSalarys.FirstOrDefaultAsync(x => x.ProjectId == row.Id);
                //if (contingency != null && engineering != null)
                //{
                //    engineering?.Percentage = request.PercentageEngineering;
                //    contingency?.Percentage = request.PercentageContingency;
                //}


                //if (!row.IsProductiveAsset)
                //{
                //    var tax = await _context.Taxes.FirstOrDefaultAsync(x => x.ProjectId == row.Id);
                //    tax!.Percentage = request.PercentageTaxProductive;
                //}
                //else if (request.IsProductiveAsset)
                //{
                //    var existingTax = await _context.Taxes.FirstOrDefaultAsync(x => x.ProjectId == row.Id);
                //    if (existingTax != null)
                //    {
                //        _context.Taxes.Remove(existingTax);
                //    }
                //    row.IsProductiveAsset = true;
                //}



                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    //await engCalculation.CalculateEngineeringTotalCost(row.Id);
                    var cacheKeyProjectDashBoardById = $"{typeof(GetProjectDashBoardStartById).Name}-{row.Id}";
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
            app.MapPost("GetInitialProjectDate", async (GetInitialProjectDate request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetInitialProjectDate).Name}-{request.ProjectId}";

                var project = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                    .Where(x => x.Id == request.ProjectId)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().FirstOrDefaultAsync();

                }, cacheKey);
                if (project == null)
                {
                    return Results.Ok(new GeneralDto<DateTime>()
                    {
                        Succeeded = false,
                        Message = "Project not found"
                    });
                }
                return Results.Ok(new GeneralDto<DateTime?>()
                {
                    Succeeded = true,
                    Data = project?.StartDate
                });
            });
        }
    }
}
