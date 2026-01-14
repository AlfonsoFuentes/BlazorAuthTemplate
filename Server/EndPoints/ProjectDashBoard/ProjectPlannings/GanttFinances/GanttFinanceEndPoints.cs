using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems;
using Server.EndPoints.ProjectDashBoard.ProjectPlannings.Gantts;
using Server.Interfaces.EndPoints;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.General;
using Shared.Dtos.Projects._2._Plannings.BudgetItemGanttTasks;
using Shared.ExtensionsMethods;

namespace Server.EndPoints.ProjectDashBoard.ProjectPlannings.GanttFinances
{
    public class GanttFinanceEndPoints : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // 1. OBTENER ASIGNACIONES DE UNA TAREA
            app.MapPost("GetAllBudgetItemGanttTask", async (GetAllBudgetItemGanttTask request, IAppDbContext _context) =>
            {
                var assignments = await _context.Set<BudgetItemGanttTask>()
                    .Include(x => x.BudgetItem)
                    .Where(x => x.GanttTaskId == request.GanttTaskId && !x.IsDeleted)
                    .Select(bg => new BudgetItemGanttTaskDto
                    {
                        Id = bg.Id,
                        GanttTaskId = bg.GanttTaskId,

                        AmountAssigned = bg.AmountAssigned,

                        ProjectId = request.ProjectId,
                        BudgetItem = bg.BudgetItem == null ? null! : new BudgetItemDto
                        {
                            Id = bg.BudgetItem.Id,
                            Name = bg.BudgetItem.Name,
                            UnitPriceUSD = bg.BudgetItem.UnitPriceUSD,
                            Quantity = bg.BudgetItem.Quantity,
                            Category = bg.BudgetItem.Category,
                            Order = bg.BudgetItem.Order,
                            ProjectId = request.ProjectId,
                        },
                    })
                    .ToListAsync();

                return Results.Ok(new GeneralDto<List<BudgetItemGanttTaskDto>> { Succeeded = true, Data = assignments });
            });

            // 2. OBTENER ITEMS DISPONIBLES (CON SALDO) PARA ASIGNAR


            // 3. GUARDAR O EDITAR ASIGNACIÓN (CON VALIDACIÓN DE SALDO)
            app.MapPost("CreateBudgetItemGanttTask", async (CreateBudgetItemGanttTask dto, IAppDbContext _context) =>
            {
                var budgetItem = await _context.BudgetItems
                    .Include(x => x.BudgetItemGanttTasks)
                    .FirstOrDefaultAsync(x => x.Id == dto.BudgetItemId);

                if (budgetItem == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Budget Item not found" });

                // Sumamos lo asignado en el Gantt actualmente
                var spent = budgetItem.BudgetItemGanttTasks
                    .Where(x => !x.IsDeleted)
                    .Sum(x => x.AmountAssigned);

                var available = budgetItem.BudgetUSD - spent;

                if (dto.AmountAssigned > available)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"Insufficient balance. Max available: {available.ToCurrencyCulture()}"
                    });
                }

                var relation = new BudgetItemGanttTask
                {
                    Id = Guid.NewGuid(),
                    GanttTaskId = dto.GanttTaskId,
                    BudgetItemId = dto.BudgetItemId,
                    AmountAssigned = dto.AmountAssigned
                };

                await _context.Set<BudgetItemGanttTask>().AddAsync(relation);

                if (await _context.SaveChangesAsync() > 0)
                {
                    _context.InvalidateCache(GanttEndPoints.CacheKeys(dto.ProjectId, dto.GanttTaskId));
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Assignment created." });
                }

                return Results.Ok(new GeneralDto { Succeeded = false, Message = "Creation failed." });
            });
            app.MapPost("EditBudgetItemGanttTask", async (EditBudgetItemGanttTask dto, IAppDbContext _context) =>
            {
                var relation = await _context.Set<BudgetItemGanttTask>()
                    .Include(x => x.BudgetItem)
                    .ThenInclude(b => b.BudgetItemGanttTasks)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (relation == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Assignment not found" });

                // Calculamos saldo disponible EXCLUYENDO esta asignación específica
                var otherAssignmentsSum = relation.BudgetItem.BudgetItemGanttTasks
                    .Where(x => x.Id != dto.Id && !x.IsDeleted)
                    .Sum(x => x.AmountAssigned);

                var realAvailable = relation.BudgetItem.BudgetUSD - otherAssignmentsSum;

                if (dto.AmountAssigned > realAvailable)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"Insufficient balance. Max allowed: {realAvailable.ToCurrencyCulture()}"
                    });
                }

                // Actualizamos el monto
                relation.AmountAssigned = dto.AmountAssigned;

                if (await _context.SaveChangesAsync() > 0)
                {
                    _context.InvalidateCache(GanttEndPoints.CacheKeys(dto.ProjectId, dto.GanttTaskId));
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Assignment updated." });
                }

                return Results.Ok(new GeneralDto { Succeeded = false, Message = "No changes were made." });
            });

            // 4. ELIMINAR ASIGNACIÓN (SOLO EL VÍNCULO)
            app.MapPost("DeleteBudgetItemGanttTask", async (DeleteBudgetItemGanttTask request, IAppDbContext _context) =>
            {
                var relation = await _context.Set<BudgetItemGanttTask>()
         .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (relation == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Assignment not found." });

                // Borrado físico de la relación (es una tabla de unión con datos simples)
                _context.Set<BudgetItemGanttTask>().Remove(relation);

                if (await _context.SaveChangesAsync() > 0)
                {
                    _context.InvalidateCache(GanttEndPoints.CacheKeys(request.ProjectId, request.GanttTaskId));
                    return Results.Ok(new GeneralDto { Succeeded = true, Message = "Link removed." });
                }

                return Results.Ok(new GeneralDto { Succeeded = false });
            });
            app.MapPost("GetAvailableBudgetsForGantt", async (GetAvailableBudgetsForGantt request, IAppDbContext _context) =>
            {
                // 1. Obtener IDs de BudgetItems que YA están en esta tarea
                var alreadyAssignedIds = await _context.Set<BudgetItemGanttTask>()
                    .Where(x => x.GanttTaskId == request.GanttTaskId && !x.IsDeleted)
                    .Select(x => x.BudgetItemId)
                    .ToListAsync();

                // 2. Buscar items del proyecto, excluyendo los que ya están en la tarea
                var budgetItems = await _context.BudgetItems
                    .Include(x => x.BudgetItemGanttTasks)
                    .Where(x => x.ProjectId == request.ProjectId
                             && !x.IsDeleted
                             && !alreadyAssignedIds.Contains(x.Id)) // <--- FILTRO CLAVE
                    .ToListAsync();

                var availableList = budgetItems.Select(b =>
                {
                    var spentInGantt = b.BudgetItemGanttTasks.Where(x => !x.IsDeleted).Sum(x => x.AmountAssigned);

                    return new BudgetItemGanttTaskDto
                    {
                        AvailableBalance = b.BudgetUSD - spentInGantt,
                        ProjectId = b.ProjectId,
                        BudgetItem = new BudgetItemDto
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Quantity = b.Quantity,
                            UnitPriceUSD = b.UnitPriceUSD,
                            Order = b.Order,
                            Category = b.Category,
                        }
                    };
                })
                .Where(x => x.AvailableBalance > 0.01m)
                .ToList();

                return Results.Ok(new GeneralDto<List<BudgetItemGanttTaskDto>> { Succeeded = true, Data = availableList });
            });
            app.MapPost("GetBudgetItemGanttTask", async (GetBudgetItemGanttTask request, IAppDbContext _context) =>
            {
                // 1. Buscamos el ítem de presupuesto con sus datos base
                var budgetItem = await _context.BudgetItems
                    .FirstOrDefaultAsync(x => x.Id == request.BudgetItemId);

                if (budgetItem == null) return Results.NotFound();

                // 2. Calculamos cuánto han gastado TODAS LAS DEMÁS tareas (excluyendo esta)
                var spentByOthers = await _context.Set<BudgetItemGanttTask>()
                    .Where(x => x.BudgetItemId == request.BudgetItemId
                             && x.GanttTaskId != request.GanttTaskId
                             && !x.IsDeleted)
                    .SumAsync(x => (decimal?)x.AmountAssigned) ?? 0;

                // 3. Buscamos la relación actual para obtener el ID y el monto guardado
                var currentRelation = await _context.Set<BudgetItemGanttTask>()
                    .FirstOrDefaultAsync(x => x.BudgetItemId == request.BudgetItemId
                                           && x.GanttTaskId == request.GanttTaskId);

                // 4. Construimos el EditBudgetItemGanttTask (o el DTO que espera tu UI)
                var dto = new EditBudgetItemGanttTask
                {
                    Id = currentRelation?.Id ?? Guid.Empty,
                    GanttTaskId = request.GanttTaskId,
                    ProjectId = budgetItem.ProjectId,
                    AmountAssigned = currentRelation?.AmountAssigned ?? 0,
                    AvailableBalance = budgetItem.BudgetUSD - spentByOthers, // Saldo REAL disponible para esta tarea
                    BudgetItem = new BudgetItemDto
                    {
                        Id = budgetItem.Id,
                        Name = budgetItem.Name,
                        Quantity = budgetItem.Quantity,
                        UnitPriceUSD = budgetItem.UnitPriceUSD,
                        Category = budgetItem.Category,
                        Order = budgetItem.Order,
                    }
                };

                return Results.Ok(new GeneralDto<EditBudgetItemGanttTask> { Succeeded = true, Data = dto });
            });
        }
    }
}
