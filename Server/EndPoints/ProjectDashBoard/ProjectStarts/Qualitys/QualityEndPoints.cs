using Server.DataContext;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.General;
using Shared.Dtos.Starts.Qualitys;
using Shared.Enums.DashBoardTable;
namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Qualitys
{

    public class QualityEndPoints : IEndPoint
    {

        void MapFromDto(QualityDto dto, Quality row)
        {
            row.Name = dto.Name;

        }
        static QualityDto MapToDto(Quality row)
        {
            QualityDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Order = row.Order;
            dto.ProjectId = row.ProjectId;
            if (row.QualityBudgetItems != null)
            {
                dto.LinkedInvestments = row.QualityBudgetItems

                        .Select(x => new BudgetItemDto
                        {
                            Id = x.BudgetItem.Id,
                            Name = x.BudgetItem.Name,
                            Quantity = x.BudgetItem.Quantity,
                            UnitPriceUSD = x.BudgetItem.UnitPriceUSD,
                            Order = x.BudgetItem.Order,
                            Category = x.BudgetItem.Category

                        }).ToList();
            }
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateQuality", async (CreateQuality dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                var row = new Quality
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                };
                MapFromDto(dto, row);
                await _context.Qualitys.AddAsync(row);



                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var maxOrder = await _context.Qualitys
                   .Where(x => x.ProjectId == dto.ProjectId)
                   .MaxAsync(x => (int?)x.Order) ?? 0;

                row.Order = maxOrder + 1;



                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    // ✅ TUS CLAVES DE CACHÉ ORIGINALES
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Quality);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Quality).Name} created successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = true, // Nota: En tu código original tenías true aquí aunque fallara, lo mantuve
                    Message = $"{typeof(Quality).Name} was not created successfully."
                });
            });


            // ✅ EDITAR
            app.MapPost("EditQuality", async (EditQuality dto, IAppDbContext _context) =>
            {
                // Incluimos relaciones para poder sincronizar
                var row = await _context.Qualitys
                    .Include(x => x.QualityBudgetItems)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Acceptance Criteria not found." // Nota: Mensaje original tuyo
                    });

                MapFromDto(dto, row);


                // ---------------------------------------------------------

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    // ✅ TUS CLAVES DE CACHÉ ORIGINALES
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Quality);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Quality).Name} Updated successfully."
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Quality).Name} was not Updated successfully."
                });
            });

            // ✅ OBTENER TODOS (Actualizado para incluir relaciones)
            app.MapPost("GetAllQualitys", async (GetAllQualitys dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllQualitys).Name}-{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Qualitys
                        .Where(x => x.ProjectId == dto.ProjectId)
                        .AsSplitQuery()
                        .AsNoTracking()
                        // 🔥 INCLUIMOS LA RELACIÓN PARA EL MAPPER
                        .Include(x => x.QualityBudgetItems)
                            .ThenInclude(qb => qb.BudgetItem)
                        .OrderBy(x => x.Order)
                        .ToListAsync();

                }, cacheKey);

                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<QualityDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });

            // ✅ OBTENER POR ID (Actualizado para incluir relaciones)
            app.MapPost("GetQualityById", async (GetQualityById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetQualityById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Qualitys
                        .AsSplitQuery()
                        .AsNoTracking()
                        // 🔥 INCLUIMOS LA RELACIÓN
                        .Include(x => x.QualityBudgetItems)
                             .ThenInclude(qb => qb.BudgetItem)
                        .AsQueryable()
                        .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);

                if (row == null)
                    return Results.Ok(new GeneralDto<QualityDto>
                    {
                        Succeeded = false,
                        Message = "Quality not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<QualityDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });

            // ... Delete, Validate, ChangeOrder (Sin cambios en lógica relacional, 
            // pero recuerda agregar cacheKeyTestings al Delete si quieres ser estricto) ...
            app.MapPost("DeleteQuality", async (DeleteQuality dto, IAppDbContext _context) =>
            {
                var row = await _context.Qualitys.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = $"{typeof(Quality).Name} was not found" });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Quality);
                    _context.InvalidateCache(keys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = $"{typeof(Quality).Name} was deleted" });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = $"{typeof(Quality).Name} was not deleted" });
            });




            // ✅ Validar nombre único
            app.MapPost("ValidateQualityName", async (ValidateQualityName dto, IAppDbContext _context) =>
                {
                    var cacheKeyAll = $"{typeof(GetAllQualitys).Name}-{dto.ProjectId}";
                    var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Qualitys.Where(x => x.ProjectId == dto.ProjectId)
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKeyAll);


                    Func<Quality, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                    var isUnique = !rows!.Any(predicate);

                    return new GeneralDto<bool>
                    {
                        Succeeded = true,
                        Data = isUnique,
                        Message = isUnique ? "Name is available." : "Name already in use."
                    };
                });
            app.MapPost("ChangeOrderQuality", async (ChangeOrderQuality dto, IAppDbContext _context) =>
            {
                var CurrentRow = await _context.Qualitys.FindAsync(dto.Id);
                if (CurrentRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Quality).Name} was not found"
                    });

                }

                var NewRow = await _context.Qualitys.Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder).FirstOrDefaultAsync();
                if (NewRow == null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Quality).Name} was not found"
                    });

                }
                NewRow.Order = CurrentRow.Order;
                CurrentRow.Order = dto.NewOrder;
                if (await _context.SaveChangesAsync() > 0)
                {


                    var cacheKeyAll = $"{typeof(GetAllQualitys).Name}-{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Quality).Name} was reorder"
                    });
                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Quality).Name} was not reorder"
                });
            });


        }
    }
}

