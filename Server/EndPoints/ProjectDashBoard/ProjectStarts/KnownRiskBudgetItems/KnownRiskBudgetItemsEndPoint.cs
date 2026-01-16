using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems;
using Server.Interfaces.EndPoints;
using Shared.Dtos.General;
using Shared.Dtos.Projects._1._Starts.KnownRiskBudgetItemDto;
using Shared.Enums.DashBoardTable;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.KnownRiskBudgetItems
{
    public class KnownRiskBudgetItemsEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("CreateKnownRiskBudgetItem", async (CreateKnownRiskBudgetItem dto, IAppDbContext _context) =>
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
                await _context.Set<KnownRiskBudgetItem>().AddAsync(new KnownRiskBudgetItem
                {
                    Id = Guid.NewGuid(),
                    KnownRiskId = dto.KnownRiskId,
                    BudgetItemId = row.Id,
                });
                if (await _context.SaveChangesAsync() > 0)
                {
                    var Budgetkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(Budgetkeys);
                    var KnownRiskkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.KnownRisks);
                    _context.InvalidateCache(KnownRiskkeys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Budget Item created successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });
            app.MapPost("EditKnownRiskBudgetItem", async (EditKnownRiskBudgetItem dto, IAppDbContext _context) =>
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
                    var KnownRiskkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.KnownRisks);
                    _context.InvalidateCache(KnownRiskkeys);

                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Budget Item created successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });
            app.MapPost("GetAllKnownRiskBudgetItem", async (GetAllKnownRiskBudgetItem dto, IAppDbContext _context) =>
            {

                var row = await _context.KnownRisks
                    .Include(x => x.KnownRiskBudgetItems)
                    .ThenInclude(qb => qb.BudgetItem)
                    .Where(qb => qb.Id == dto.KnownRiskId)

                    .FirstOrDefaultAsync();
                if (row == null)
                {
                    return Results.Ok(new GeneralDto<List<KnownRiskBudgetItemDto>>
                    {
                        Succeeded = false,
                        Message = "KnownRisk not found"
                    });
                }
                var dtos = row.KnownRiskBudgetItems.Select(qb => new KnownRiskBudgetItemDto
                {
                    Id = qb.Id,
                    KnownRiskId = qb.KnownRiskId,
                    ProjectId = qb.KnownRisk.ProjectId,
                    BudgetItemId = qb.BudgetItemId,
                    BudgetName = qb.BudgetItem.Name,
                    Category = qb.BudgetItem.Category,
                    UnitPriceUSD = qb.BudgetItem.UnitPriceUSD,
                    Quantity = qb.BudgetItem.Quantity,
                    Nomenclatore = qb.BudgetItem.Nomenclatore,


                    Order = qb.Order
                }).ToList();

                return Results.Ok(new GeneralDto<List<KnownRiskBudgetItemDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("GetByIdKnownRiskBudgetItem", async (GetByIdKnownRiskBudgetItem dto, IAppDbContext _context) =>
            {
                // Buscamos la relación específica incluyendo el ítem de presupuesto
                var qb = await _context.Set<KnownRiskBudgetItem>()
                    .Include(x => x.BudgetItem)
                    .Include(x => x.KnownRisk)
                    .FirstOrDefaultAsync(x => x.KnownRiskId == dto.KnownRiskId && x.BudgetItemId == dto.BudgetItemId);

                if (qb == null || qb.BudgetItem == null)
                {
                    return Results.Ok(new GeneralDto<KnownRiskBudgetItemDto>
                    {
                        Succeeded = false,
                        Message = "Investment link not found"
                    });
                }

                // Mapeamos al DTO que usa tu UI
                var resultDto = new KnownRiskBudgetItemDto
                {
                    Id = qb.Id,
                    KnownRiskId = qb.KnownRiskId,
                    BudgetItemId = qb.BudgetItemId,
                    ProjectId = qb.KnownRisk.ProjectId,
                    BudgetName = qb.BudgetItem.Name,
                    Category = qb.BudgetItem.Category,
                    Quantity = qb.BudgetItem.Quantity,
                    UnitPriceUSD = qb.BudgetItem.UnitPriceUSD,
                    Order = qb.Order,
                    KnownRiskName = qb.KnownRisk.Name,
                    Nomenclatore = qb.BudgetItem.Nomenclatore // Asegúrate de tener este helper o propiedad
                };

                return Results.Ok(new GeneralDto<KnownRiskBudgetItemDto>
                {
                    Succeeded = true,
                    Data = resultDto
                });
            });

            app.MapPost("DeleteKnownRiskBudgetItem", async (DeleteKnownRiskBudgetItem dto, IAppDbContext _context) =>
            {
                var row = await _context.BudgetItems.FindAsync(dto.BudgetItemId);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found" });

                var category = row.Category;
                row.IsDeleted = true;
                var relations = await _context.Set<KnownRiskBudgetItem>()
                    .Where(x => x.BudgetItemId == dto.BudgetItemId)
                    .ToListAsync();

                if (relations.Any())
                {
                    _context.Set<KnownRiskBudgetItem>().RemoveRange(relations);
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
                    var KnownRiskkeys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.KnownRisks);
                    _context.InvalidateCache(KnownRiskkeys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Deleted successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });
        }
    }
}
