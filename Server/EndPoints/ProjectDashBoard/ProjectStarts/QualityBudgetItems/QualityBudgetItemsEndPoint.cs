using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems;
using Server.Interfaces.EndPoints;
using Shared.Dtos.General;
using Shared.Dtos.Projects._1._Starts.QualityBudgetItems;
using Shared.Dtos.Starts.Qualitys;
using Shared.Enums.DashBoardTable;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.QualityBudgetItems
{
    public class QualityBudgetItemsEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("CreateQualityBudgetItem", async (CreateQualityBudgetItem dto, IAppDbContext _context) =>
            {
                var row = new BudgetItem
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                    Name = dto.BudgetName,
                    Category = dto.Category,
                    Quantity = dto.Quantity,
                    UnitPriceUSD = dto.UnitPriceUSD,
                    // Persistencia segura
                    BudgetUSD = (decimal)dto.Quantity * dto.UnitPriceUSD
                };

                await _context.BudgetItems.AddAsync(row);

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null) project.LastModifiedOn = DateTime.UtcNow;

                var maxOrder = await _context.BudgetItems
                    .Where(x => x.ProjectId == dto.ProjectId && x.Category == dto.Category)
                    .MaxAsync(x => (int?)x.Order) ?? 0;

                row.Order = maxOrder + 1;
                await _context.Set<QualityBudgetItem>().AddAsync(new QualityBudgetItem
                {
                    Id = Guid.NewGuid(),
                    QualityId = dto.QualityId,
                    BudgetItemId = row.Id,
                });
                if (await _context.SaveChangesAsync() > 0)
                {
                    var Budgetkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(Budgetkeys);
                    var Qualitykeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Quality);
                    _context.InvalidateCache(Qualitykeys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Budget Item created successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });
            app.MapPost("EditQualityBudgetItem", async (EditQualityBudgetItem dto, IAppDbContext _context) =>
            {
                var row = await _context.BudgetItems.FindAsync(dto.BudgetItemId);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found" });
                row.Name = dto.BudgetName;
                var oldCategory = row.Category;
                var categoryChanged = oldCategory != dto.Category;

                row.Category = dto.Category;
                row.Quantity = dto.Quantity;
                row.UnitPriceUSD = dto.UnitPriceUSD;
                // Persistencia segura
                row.BudgetUSD = (decimal)dto.Quantity * dto.UnitPriceUSD;
                if (categoryChanged)
                {
                    // 1. Asignar nuevo orden al final de la nueva categoría
                    var maxOrder = await _context.BudgetItems
                        .Where(x => x.ProjectId == dto.ProjectId && x.Category == dto.Category && !x.IsDeleted)
                        .MaxAsync(x => (int?)x.Order) ?? 0;

                    row.Order = maxOrder + 1;
                }

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project != null) project.LastModifiedOn = DateTime.UtcNow;

               
                if (await _context.SaveChangesAsync() > 0)
                {
                    if (categoryChanged)
                    {
                        var remainingOldCat = await _context.BudgetItems
                            .Where(x => x.ProjectId == dto.ProjectId && x.Category == oldCategory && !x.IsDeleted)
                            .OrderBy(x => x.Order)
                            .ToListAsync();

                        int i = 1;
                        foreach (var item in remainingOldCat) { item.Order = i++; }
                        await _context.SaveChangesAsync();
                    }
                    var Budgetkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(Budgetkeys);
                    var Qualitykeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Quality);
                    _context.InvalidateCache(Qualitykeys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Budget Item created successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });
            app.MapPost("GetAllQualityBudgetItem", async (GetAllQualityBudgetItem dto, IAppDbContext _context) =>
            {

                var row = await _context.Qualitys
                    .Include(x => x.QualityBudgetItems)
                    .ThenInclude(qb => qb.BudgetItem)
                    .Where(qb => qb.Id == dto.QualityId)

                    .FirstOrDefaultAsync();
                if (row == null)
                {
                    return Results.Ok(new GeneralDto<List<QualityBudgetItemDto>>
                    {
                        Succeeded = false,
                        Message = "Quality not found"
                    });
                }
                var dtos = row.QualityBudgetItems.Select(qb => new QualityBudgetItemDto
                {
                    Id = qb.Id,
                    QualityId = qb.QualityId,
                    ProjectId = qb.Quality.ProjectId,
                    BudgetItemId = qb.BudgetItemId,
                    BudgetName = qb.BudgetItem.Name,
                    Category = qb.BudgetItem.Category,
                    UnitPriceUSD = qb.BudgetItem.UnitPriceUSD,
                    Quantity = qb.BudgetItem.Quantity,
                    Nomenclatore = qb.BudgetItem.Nomenclatore,


                    Order = qb.Order
                }).ToList();

                return Results.Ok(new GeneralDto<List<QualityBudgetItemDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("GetByIdQualityBudgetItem", async (GetByIdQualityBudgetItem dto, IAppDbContext _context) =>
            {
                // Buscamos la relación específica incluyendo el ítem de presupuesto
                var qb = await _context.Set<QualityBudgetItem>()
                    .Include(x => x.BudgetItem)
                    .Include(x => x.Quality)
                    .FirstOrDefaultAsync(x => x.QualityId == dto.QualityId && x.BudgetItemId == dto.BudgetItemId);

                if (qb == null || qb.BudgetItem == null)
                {
                    return Results.Ok(new GeneralDto<QualityBudgetItemDto>
                    {
                        Succeeded = false,
                        Message = "Investment link not found"
                    });
                }

                // Mapeamos al DTO que usa tu UI
                var resultDto = new QualityBudgetItemDto
                {
                    Id = qb.Id,
                    QualityId = qb.QualityId,
                    BudgetItemId = qb.BudgetItemId,
                    ProjectId = qb.Quality.ProjectId,
                    BudgetName = qb.BudgetItem.Name,
                    Category = qb.BudgetItem.Category,
                    Quantity = qb.BudgetItem.Quantity,
                    UnitPriceUSD = qb.BudgetItem.UnitPriceUSD,
                    Order = qb.Order,
                    QualityName = qb.Quality.Name,
                    Nomenclatore = qb.BudgetItem.Nomenclatore // Asegúrate de tener este helper o propiedad
                };

                return Results.Ok(new GeneralDto<QualityBudgetItemDto>
                {
                    Succeeded = true,
                    Data = resultDto
                });
            });

            app.MapPost("DeleteQualityBudgetItem", async (DeleteQualityBudgetItem dto, IAppDbContext _context) =>
            {
                var row = await _context.BudgetItems.FindAsync(dto.BudgetItemId);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found" });

                var category = row.Category;
                row.IsDeleted = true;
                var relations = await _context.Set<QualityBudgetItem>()
                    .Where(x => x.BudgetItemId == dto.BudgetItemId)
                    .ToListAsync();

                if (relations.Any())
                {
                    _context.Set<QualityBudgetItem>().RemoveRange(relations);
                }
                if (await _context.SaveChangesAsync() > 0)
                {
                    // Reordenar dentro de la categoría
                    var remaining = await _context.BudgetItems
                        .Where(x => x.ProjectId == dto.ProjectId && x.Category == category && !x.IsDeleted)
                        .OrderBy(x => x.Order)
                        .ToListAsync();

                    int i = 1;
                    foreach (var item in remaining) { item.Order = i++; }
                    await _context.SaveChangesAsync();

                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(keys);
                    var Qualitykeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Quality);
                    _context.InvalidateCache(Qualitykeys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Deleted successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });
        }
    }
}
