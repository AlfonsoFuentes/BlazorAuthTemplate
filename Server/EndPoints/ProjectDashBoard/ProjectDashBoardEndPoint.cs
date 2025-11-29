using Azure.Core;
using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams;
using Server.Interfaces.EndPoints;
using Server.Services;
using Shared.Dtos.Brands;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Suppliers;

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

            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("GetAllProjectDashBoards", async (GetAllProjectDashBoards dto, IAppDbContext _context, ICache _cache) =>
            {
                var cacheKey = $"{typeof(GetAllProjectDashBoards).Name}";
                var rows = await _cache.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToProjectDashboardDto).ToList();

                return Results.Ok(new GeneralDto<List<ProjectDashboardDto>>
                {
                    Suceeded = true,
                    Data = dtos
                });
            });
            app.MapPost("GetProjectDashBoardById", async (GetProjectDashBoardById request, IAppDbContext _context, ICache _cache) =>
            {
                var cacheKey = $"{typeof(GetProjectDashBoardById).Name}-{request.Id}";
                var row = await _cache.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);



                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Suceeded = false,
                        Message = $"{typeof(Project).Name} was not found"
                    });
                }

                var dto = MapToProjectDashboardDto(row);
                return Results.Ok(new GeneralDto<ProjectDashboardDto>
                {
                    Suceeded = true,
               
                    Data = dto


                });
            });

           
        }
    }
}
