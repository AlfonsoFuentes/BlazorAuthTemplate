using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.ExpertJudgements;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.ExpertJudgements
{

    public class ExpertJudgementEndPoints : IEndPoint
    {

        void MapFromDto(ExpertJudgementDto dto, ExpertJudgement row)
        {
            row.Name = dto.Name;
            row.ExpertId = dto.Expert != null ? dto.Expert.Id : null;

        }
        static ExpertJudgementDto MapToDto(ExpertJudgement row)
        {
            ExpertJudgementDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            dto.Expert = row.Expert != null ? new StakeHolderDto
            {
                Id = row.Expert!.Id,
                Name = row.Expert.Name,
                Email = row.Expert.Email,
                PhoneNumber = row.Expert.PhoneNumber,
                Area = row.Expert.Area

            } : null;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateExpertJudgement", async (CreateExpertJudgement dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new ExpertJudgement
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.ExpertJudgements.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var cacheKeyAll = $"{typeof(GetAllExpertJudgements).Name}{dto.ProjectId}";
               
                var maxOrder = await getNextOrder.GetNextOrderAsync<ExpertJudgement>(cacheKeyAll, dto.ProjectId);
                row.Order = maxOrder;

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById,cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(ExpertJudgement).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true,
                    Message = $"{typeof(ExpertJudgement).Name} was not created successfully."
                });


            });


            // ✅ Editar
            app.MapPost("EditExpertJudgement", async (EditExpertJudgement dto, IAppDbContext _context) =>
            {
                var row = await _context.ExpertJudgements.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Acceptance Criteria not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyId = $"{typeof(GetExpertJudgementById).Name}-{dto.Id}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyAll = $"{typeof(GetAllExpertJudgements).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(ExpertJudgement).Name} Updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(ExpertJudgement).Name} was not Updated successfully."
                });


            });

            // ✅ Obtener por ID
            app.MapPost("GetExpertJudgementById", async (GetExpertJudgementById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetExpertJudgementById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.ExpertJudgements
                    .Include(x => x.Expert)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);



                if (row == null)
                    return Results.Ok(new GeneralDto<ExpertJudgementDto>
                    {
                        Succeeded = false,
                        Message = "Expert was  not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<ExpertJudgementDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllExpertJudgements", async (GetAllExpertJudgements dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllExpertJudgements).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.ExpertJudgements
                    .Include(x => x.Expert)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .Where(x => x.ProjectId == dto.ProjectId)
                  .OrderBy(x => x.Order)
                  .ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<ExpertJudgementDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteExpertJudgement", async (DeleteExpertJudgement dto, IAppDbContext _context) =>
            {
                var row = await _context.ExpertJudgements.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(ExpertJudgement).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var datas = await _context.ExpertJudgements.Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                    int i = 1;
                    foreach (var data in datas)
                    {
                        data.Order = i;
                        i++;
                    }
                    await _context.SaveChangesAsync();
                    var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectChartedPDF).Name}-{dto.ProjectId}";
                    var cacheKeyAll = $"{typeof(GetAllExpertJudgements).Name}{dto.ProjectId}";
                    var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
                    var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardById).Name}-{dto.ProjectId}";
                    _context.InvalidateCache(cacheKeyAll, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById, cacheKeyExportProjectCharterPDF);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(ExpertJudgement).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(ExpertJudgement).Name} was not deleted"
                });
            });


            // ✅ Validar nombre único
            app.MapPost("ValidateExpertJudgementName", async (ValidateExpertJudgementName dto, IAppDbContext _context) =>
            {
                var cacheKeyAll = $"{typeof(GetAllExpertJudgements).Name}{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.ExpertJudgements
                    .Include(x => x.Expert)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                Func<ExpertJudgement, bool> predicate = x => dto.Id == Guid.Empty ? x.ExpertId==dto.ExpertId : x.Id != dto.Id && x.ExpertId==dto.ExpertId;

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ChangeOrderExpertJudgement", async (ChangeOrderExpertJudgement dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.ExpertJudgements.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(ExpertJudgement).Name} was not found"
                    });

                }

                var NewRow = await _context.ExpertJudgements.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(ExpertJudgement).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllExpertJudgements).Name}{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(ExpertJudgement).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(ExpertJudgement).Name} was not reorder"
                });
            });


        }
    }
}

