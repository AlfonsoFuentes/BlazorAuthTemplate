using Microsoft.EntityFrameworkCore;
using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems;
using Server.Domain.CommonEntities.ProjectManagements;
using Server.Interfaces.EndPoints;
using Shared.Dtos;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Starts.ExpertJudgements;
using Shared.Enums.BudgetCategorys;
using Shared.Enums.DashBoardTable;

namespace Server.Endpoints
{
    public class BudgetItemEndPoints : IEndPoint
    {
        static void MapFromDto(BudgetItemDto dto, BudgetItem row)
        {
            row.Name = dto.Name;
            row.Category = dto.Category;
            row.Quantity = dto.Quantity;
            row.UnitPriceUSD = dto.UnitPriceUSD;

            // ✅ Cálculo seguro casteando a decimal
            row.BudgetUSD = (decimal)dto.Quantity * dto.UnitPriceUSD;
        }

        static BudgetItemDto MapToDto(BudgetItem row)
        {
            return new BudgetItemDto
            {
                Id = row.Id,
                ProjectId = row.ProjectId,
                Name = row.Name,
                Order = row.Order,
                Category = row.Category,
                Quantity = row.Quantity,
                UnitPriceUSD = row.UnitPriceUSD
                // BudgetUSD se calcula solo en el DTO
            };
        }

        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ CREATE
            app.MapPost("CreateBudgetItem", async (CreateBudgetItem dto, IAppDbContext _context) =>
            {
                var row = new BudgetItem
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                    Name = dto.Name,
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

                if (await _context.SaveChangesAsync() > 0)
                {
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Budget Item created successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });

            // ✅ EDIT
            app.MapPost("EditBudgetItem", async (EditBudgetItem dto, IAppDbContext _context) =>
            {
                var row = await _context.BudgetItems.FindAsync(dto.Id);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found." });

                MapFromDto(dto, row);

                if (await _context.SaveChangesAsync() > 0)
                {
                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Updated successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Update failed." });
            });

            // ✅ GET ALL
            app.MapPost("GetAllBudgetItems", async (GetAllBudgetItems dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllBudgetItems).Name}-{dto.ProjectId}";

                var project = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                        .Include(p => p.BudgetItems)
                        .AsNoTracking()
                        .AsSplitQuery()
                        .Where(x => x.Id == dto.ProjectId)
                        .FirstOrDefaultAsync();
                }, cacheKey);

                if (project == null)
                    return Results.Ok(new GeneralDto<List<BudgetItemDto>> { Succeeded = false, Message = "Project not found!" });

                var dtos = project.BudgetItems
                    .OrderBy(x => x.Category)
                    .ThenBy(x => x.Order)
                    .Select(MapToDto)
                    .ToList();

                // --- CALCULOS FINANCIEROS (Decimales) ---

                // Suma total en Decimal
                decimal subTotal = dtos.Where(x => x.Category != BudgetCategory.Alteration).Sum(x => x.BudgetUSD);

                // Porcentajes (asumiendo que en BD son double, casteamos a decimal)
                decimal pctTax = (decimal)project.PercentageTaxProductive;
                decimal pctContingency = (decimal)project.PercentageContingency;
                decimal pctEngineering = (double)project.PercentageEngineering > 0 ? (decimal)project.PercentageEngineering : 0;

                // 1. TAXES
                if (!project.IsProductiveAsset)
                {
                    decimal taxAmount = subTotal * (pctTax / 100m);

                    dtos.Add(new BudgetItemDto
                    {
                        Id = Guid.Empty,
                        ProjectId = dto.ProjectId,
                        Name = $"Taxes {project.PercentageTaxProductive}%",
                        Category = BudgetCategory.Tax,
                        Quantity = 1,
                        UnitPriceUSD = taxAmount,
                        Order = 1
                    });
                    subTotal = dtos.Sum(x => x.BudgetUSD);
                }
                var sumEngCont = pctContingency + pctEngineering;
                var divisor = 100 - sumEngCont;
                // 2. ITEMS ESPECIALES (Contingencia e Ingeniería)
                // Usamos la lógica de porcentaje directo sobre el subtotal
                if (pctContingency > 0)
                {
                    decimal contAmount = (subTotal * pctContingency) / divisor;
                    dtos.Add(new BudgetItemDto
                    {
                        Id = Guid.Empty,
                        ProjectId = dto.ProjectId,
                        Name = $"Contingency {project.PercentageContingency}%",
                        Category = BudgetCategory.Contingency,
                        Quantity = 1,
                        UnitPriceUSD = contAmount,
                        Order = 1
                    });
                }

                if (pctEngineering > 0)
                {
                    decimal engAmount = (subTotal * pctEngineering) / divisor;
                    dtos.Add(new BudgetItemDto
                    {
                        Id = Guid.Empty,
                        ProjectId = dto.ProjectId,
                        Name = $"Engineering {project.PercentageEngineering}%",
                        Category = BudgetCategory.Engineering,
                        Quantity = 1,
                        UnitPriceUSD = engAmount,
                        Order = 1
                    });
                }

                return Results.Ok(new GeneralDto<List<BudgetItemDto>>
                {
                    Succeeded = true,
                    Data = dtos.OrderBy(x => x.Nomenclatore).ToList()
                });
            });

            // ✅ GET BY ID
           
            app.MapPost("GetBudgetItemById", async (GetBudgetItemById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetBudgetItemById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.BudgetItems
                   
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);



                if (row == null)
                    return Results.Ok(new GeneralDto<BudgetItemDto>
                    {
                        Succeeded = false,
                        Message = "Expert was  not found."
                    });

                var dto = MapToDto(row);

                return Results.Ok(new GeneralDto<BudgetItemDto>
                {
                    Succeeded = true,
                    Data = dto
                });
            });
            // ✅ DELETE
            app.MapPost("DeleteBudgetItem", async (DeleteBudgetItem dto, IAppDbContext _context) =>
            {
                var row = await _context.BudgetItems.FindAsync(dto.Id);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Not found" });

                var category = row.Category;
                row.IsDeleted = true;

                if (await _context.SaveChangesAsync() > 0)
                {
                    // Reordenar dentro de la categoría
                    var remaining = await _context.BudgetItems
                        .Where(x => x.ProjectId == dto.ProjectId && x.Category == category)
                        .OrderBy(x => x.Order)
                        .ToListAsync();

                    int i = 1;
                    foreach (var item in remaining) { item.Order = i++; }
                    await _context.SaveChangesAsync();

                    var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.Investment);
                    _context.InvalidateCache(keys);
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Deleted successfully." });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });

            // ✅ CHANGE ORDER
            app.MapPost("ChangeOrderBudgetItem", async (ChangeOrderBudgetItem dto, IAppDbContext _context) =>
            {
                var current = await _context.BudgetItems.FindAsync(dto.Id);
                if (current == null) return Results.Ok(new GeneralDto { Succeeded = false });

                var target = await _context.BudgetItems
                    .Where(x => x.ProjectId == dto.ProjectId
                             && x.Category == dto.Category
                             && x.Order == dto.NewOrder)
                    .FirstOrDefaultAsync();

                if (target != null)
                {
                    target.Order = current.Order;
                    current.Order = dto.NewOrder;
                    await _context.SaveChangesAsync();
                    var cacheKeyAll = $"{typeof(GetAllBudgetItems).Name}-{dto.ProjectId}";

                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto { Succeeded = true });
                }
                return Results.Ok(new GeneralDto { Succeeded = false });
            });

            // ✅ VALIDATE NAME
            app.MapPost("ValidateBudgetItemName", async (ValidateBudgetItemName dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllBudgetItems).Name}-{dto.ProjectId}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.BudgetItems.AsNoTracking()
                        .Where(x => x.ProjectId == dto.ProjectId).ToListAsync();
                }, cacheKey);

                Func<BudgetItem, bool> predicate = x =>
                   x.Category == dto.Category &&
                   (dto.Id == Guid.Empty
                       ? x.Name.Trim().Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                       : x.Id != dto.Id && x.Name.Trim().Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase));

                var isUnique = !rows!.Any(predicate);
                return new GeneralDto<bool> { Succeeded = true, Data = isUnique , Message = isUnique ? "Name is available." : "Name already in use." };
            });
        }

        private void InvalidateCaches(IAppDbContext _context, Guid projectId)
        {
            var cacheKeyList = $"{typeof(GetAllBudgetItems).Name}-{projectId}";
            var cacheKeyProjectDashBoards = $"{typeof(GetAllProjectDashBoards).Name}";
            var cacheKeyProjectDashBoardsById = $"{typeof(GetProjectDashBoardStartById).Name}-{projectId}";
            _context.InvalidateCache(cacheKeyList, cacheKeyProjectDashBoards, cacheKeyProjectDashBoardsById);
        }
    }
}