using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems;
using Server.Interfaces.EndPoints;
using Shared.Dtos.General;
using Shared.Dtos.Projects._1._Starts.RiskMatrixBudgetItemDto;
using Shared.Enums.DashBoardTable;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.RiskMatrixBudgetItems
{
    public class RiskMatrixBudgetItemsEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("CreateRiskMatrixBudgetItem", async (CreateRiskMatrixBudgetItem dto, IAppDbContext _context) =>
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
                await _context.Set<RiskBudgetItem>().AddAsync(new RiskBudgetItem
                {
                    Id = Guid.NewGuid(),
                    RiskMatrixId = dto.RiskMatrixId,
                    BudgetItemId = row.Id,
                });
                if (await _context.SaveChangesAsync() > 0)
                {
                    var Budgetkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(Budgetkeys);
                    var RiskMatrixkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                    _context.InvalidateCache(RiskMatrixkeys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Budget Item created successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });
            app.MapPost("EditRiskMatrixBudgetItem", async (EditRiskMatrixBudgetItem dto, IAppDbContext _context) =>
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
                    var RiskMatrixkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                    _context.InvalidateCache(RiskMatrixkeys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Budget Item created successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });
            app.MapPost("GetAllRiskMatrixBudgetItem", async (GetAllRiskMatrixBudgetItem dto, IAppDbContext _context) =>
            {

                var row = await _context.RiskMatrixs
                    .Include(x => x.RiskBudgetItems)
                    .ThenInclude(qb => qb.BudgetItem)
                    .Where(qb => qb.Id == dto.RiskMatrixId)

                    .FirstOrDefaultAsync();
                if (row == null)
                {
                    return Results.Ok(new GeneralDto<List<RiskMatrixBudgetItemDto>>
                    {
                        Succeeded = false,
                        Message = "RiskMatrix not found"
                    });
                }
                var dtos = row.RiskBudgetItems.Select(qb => new RiskMatrixBudgetItemDto
                {
                    Id = qb.Id,
                    RiskMatrixId = qb.RiskMatrixId,
                    ProjectId = qb.RiskMatrix.ProjectId,
                    BudgetItemId = qb.BudgetItemId,
                    BudgetName = qb.BudgetItem.Name,
                    Category = qb.BudgetItem.Category,
                    UnitPriceUSD = qb.BudgetItem.UnitPriceUSD,
                    Quantity = qb.BudgetItem.Quantity,
                    Nomenclatore = qb.BudgetItem.Nomenclatore,


                    Order = qb.Order
                }).ToList();

                return Results.Ok(new GeneralDto<List<RiskMatrixBudgetItemDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("GetByIdRiskMatrixBudgetItem", async (GetByIdRiskMatrixBudgetItem dto, IAppDbContext _context) =>
            {
                // Buscamos la relación específica incluyendo el ítem de presupuesto
                var qb = await _context.Set<RiskBudgetItem>()
                    .Include(x => x.BudgetItem)
                    .Include(x => x.RiskMatrix)
                    .FirstOrDefaultAsync(x => x.RiskMatrixId == dto.RiskMatrixId && x.BudgetItemId == dto.BudgetItemId);

                if (qb == null || qb.BudgetItem == null)
                {
                    return Results.Ok(new GeneralDto<RiskMatrixBudgetItemDto>
                    {
                        Succeeded = false,
                        Message = "Investment link not found"
                    });
                }

                // Mapeamos al DTO que usa tu UI
                var resultDto = new RiskMatrixBudgetItemDto
                {
                    Id = qb.Id,
                    RiskMatrixId = qb.RiskMatrixId,
                    BudgetItemId = qb.BudgetItemId,
                    ProjectId = qb.RiskMatrix.ProjectId,
                    BudgetName = qb.BudgetItem.Name,
                    Category = qb.BudgetItem.Category,
                    Quantity = qb.BudgetItem.Quantity,
                    UnitPriceUSD = qb.BudgetItem.UnitPriceUSD,
                    Order = qb.Order,
                    RiskMatrixName = qb.RiskMatrix.Title,
                    Nomenclatore = qb.BudgetItem.Nomenclatore // Asegúrate de tener este helper o propiedad
                };

                return Results.Ok(new GeneralDto<RiskMatrixBudgetItemDto>
                {
                    Succeeded = true,
                    Data = resultDto
                });
            });

            app.MapPost("DeleteRiskMatrixBudgetItem", async (DeleteRiskMatrixBudgetItem dto, IAppDbContext _context) =>
            {
                var row = await _context.BudgetItems.FindAsync(dto.BudgetItemId);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found" });

                var category = row.Category;
                row.IsDeleted = true;
                var relations = await _context.Set<RiskBudgetItem>()
                    .Where(x => x.BudgetItemId == dto.BudgetItemId)
                    .ToListAsync();

                if (relations.Any())
                {
                    _context.Set<RiskBudgetItem>().RemoveRange(relations);
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
                    var RiskMatrixkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                    _context.InvalidateCache(RiskMatrixkeys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Deleted successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });
        }
    }
}
