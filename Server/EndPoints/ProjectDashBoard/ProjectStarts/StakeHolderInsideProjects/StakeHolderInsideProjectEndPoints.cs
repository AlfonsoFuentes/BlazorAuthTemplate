using Microsoft.EntityFrameworkCore.Query;
using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Domain.CommonEntities.ProjectManagements;
using Server.Interfaces.EndPoints;
using Server.Services;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.StakeHolderInsideProjectInsideProjects;
using Shared.Enums.StakeHolderTypes;
using System.Linq.Expressions;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.StakeHolderInsideProjects
{

    public class StakeHolderInsideProjectEndPoints : IEndPoint
    {


        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateStakeHolderInsideProject", async (CreateStakeHolderInsideProject dto, IAppDbContext _context) =>
            {


                var project = await _context.Projects
                .FirstOrDefaultAsync(x => x.Id == dto.ProjectId);

                if (project == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"Project was not found!"
                    });
                }
                var stakeholder = await _context.StakeHolders
                .Include(x => x.RoleInsideProject)
                .FirstOrDefaultAsync(x => x.Id == dto.StakeHolderId);
                if (stakeholder == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"StakeHolder was not found!"
                    });
                }
                var rolesinsideproject = await _context.RoleInsideProjects.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                if (rolesinsideproject.Any(x => x.Name.Equals(dto.Role.Name)))
                {
                    var roleinsideProject = rolesinsideproject.First(x => x.Name.Equals(dto.Role.Name));
                    stakeholder.RoleInsideProjectId = roleinsideProject.Id;
                }
                else
                {
                    RoleInsideProject roleInsideProject = RoleInsideProject.Create(project.Id, dto.Role.Name);
                    await _context.RoleInsideProjects.AddAsync(roleInsideProject);
                    stakeholder.RoleInsideProjectId = roleInsideProject.Id;
                }
                project.StakeHolders.Add(stakeholder);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    var cacheKeyAll = $"{typeof(GetAllStakeHolderInsideProjects).Name}{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"Stakeholder inside project was added successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"Stakeholder inside project was not created successfully."
                });


            });


            // ✅ Editar
            app.MapPost("EditStakeHolderInsideProject", async (EditStakeHolderInsideProject dto, IAppDbContext _context) =>
            {
                var project = await _context.Projects
                .Include(x => x.StakeHolders)
                .ThenInclude(x => x.RoleInsideProject)
                .FirstOrDefaultAsync(x => x.Id == dto.ProjectId);

                if (project == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"Project was not found!"
                    });
                }
                var stakeholder = project.StakeHolders.FirstOrDefault(x => x.Id == dto.StakeHolderId);
                if (stakeholder == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"StakeHolder was not found!"
                    });
                }
                var rolesinsideproject = await _context.RoleInsideProjects.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                if (rolesinsideproject.Any(x => x.Name.Equals(dto.Role.Name)))
                {
                    var roleinsideProject = rolesinsideproject.First(x => x.Name.Equals(dto.Role.Name));
                    stakeholder.RoleInsideProjectId = roleinsideProject.Id;
                }
                else
                {
                    RoleInsideProject roleInsideProject = RoleInsideProject.Create(project.Id, dto.Role.Name);
                    await _context.RoleInsideProjects.AddAsync(roleInsideProject);
                    stakeholder.RoleInsideProjectId = roleInsideProject.Id;
                }
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyId = $"{typeof(GetStakeHolderInsideProjectById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllStakeHolderInsideProjects).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById,cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"Stakeholder inside project Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"Stakeholder inside project was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetStakeHolderInsideProjectById", async (GetStakeHolderInsideProjectById request, IAppDbContext _context) =>
            {
                var project = await _context.Projects
                 .Include(x => x.StakeHolders)
                 .ThenInclude(x => x.RoleInsideProject)
                 .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

                if (project == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"Project was not found!"
                    });
                }
                var stakeholder = project.StakeHolders.FirstOrDefault(x => x.Id == request.StakeHolderId);

                if (stakeholder == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"StakeHolder was not found!"
                    });
                }

                StakeHolderInsideProjectDto dto = new()
                {
                    ProjectId = project.Id,
                 
                    Role = StakeHolderRoleEnum.GetType(stakeholder.RoleInsideProject!.Name)   ,
                    StakeHolderInsideProject = new StakeHolderDto()
                    {
                        Id = stakeholder.Id,
                        Area = stakeholder.Area,
                        Email = stakeholder.Email,
                        PhoneNumber = stakeholder.PhoneNumber,
                        Name = stakeholder.Name,
                    },

                };
                return Results.Ok(new GeneralDto<StakeHolderInsideProjectDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllStakeHolderInsideProjects", async (GetAllStakeHolderInsideProjects dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllStakeHolderInsideProjects).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                    .Include(x => x.StakeHolders)
                    .ThenInclude(x => x.RoleInsideProject)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.Id == dto.ProjectId)

                  .FirstOrDefaultAsync();

                }, cacheKey);

                if (rows == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"Project was not found!"
                    });
                }

                var dtos = rows.StakeHolders.Select(x => new StakeHolderInsideProjectDto()
                {
                    ProjectId = dto.ProjectId,
                    StakeHolderInsideProject = new StakeHolderDto()
                    {
                        Id = x.Id,
                        Area = x.Area,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Name = x.Name,
                    },
                    Role = StakeHolderRoleEnum.GetType(x.RoleInsideProject!.Name),
             


                }).ToList();

                return Results.Ok(new GeneralDto<List<StakeHolderInsideProjectDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteStakeHolderInsideProject", async (DeleteStakeHolderInsideProject dto, IAppDbContext _context) =>
            {
                var row = await _context.Projects
                .Include(x => x.StakeHolders)
                .FirstOrDefaultAsync(x => x.Id == dto.ProjectId);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Project).Name} was not found"
                    });
                }
                var stakeholder = row.StakeHolders.FirstOrDefault(x => x.Id == dto.StakeHolderId);
                if (stakeholder is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"Stakeholder was not found!"
                    });
                }
                row.StakeHolders.Remove(stakeholder);

                if (await _context.SaveChangesAsync() > 0)
                {

                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyAll = $"{typeof(GetAllStakeHolderInsideProjects).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(StakeHolder).Name} was deleted in Project"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(StakeHolder).Name} was not deleted in Project"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateStakeHolderInsideProjectName", async (ValidateStakeHolderInsideProjectName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllStakeHolderInsideProjects).Name}{dto.ProjectId}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                    .Include(x => x.StakeHolders)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().FirstOrDefaultAsync(x => x.Id == dto.ProjectId);

                }, cacheKeyAll);




                var isUnique = row!.StakeHolders.Any(x => x.Id == dto.StakeHolderId);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Stakeholder is available." : "Stakeholder already in use."
                };
            });


        }
    }
}

