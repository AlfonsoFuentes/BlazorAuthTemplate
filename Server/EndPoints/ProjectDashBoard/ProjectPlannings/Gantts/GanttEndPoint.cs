using Azure;
using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Dtos.Projects._2._Plannings.Gantts;
using Shared.Dtos.Projects.Plannings.Gantts;
using Shared.Enums.BudgetCategorys;
using Shared.ExtensionsMethods;

namespace Server.EndPoints.ProjectDashBoard.ProjectPlannings.Gantts
{



    public class GanttEndPoints : IEndPoint
    {
        public static string[] CacheKeys(Guid projectId, Guid taskId) => new[]
        {
                    $"GetAllGanttTaskToValidateName-{projectId}",
                    $"{typeof(GetAllGanttTasks).Name}-{projectId}",
                    $"{typeof(GetGanttTaskById).Name}-{taskId}",
                    $"{typeof(GetAllProjectDashBoards).Name}",
                    $"{typeof(GetProjectDashBoardStartById).Name}-{projectId}",
                    $"{typeof(ExportProjectPlannPDF).Name}-{projectId}"
    };
        private static void MapFromDto(GanttDto dto, GanttTask row)
        {
            row.Name = dto.Name;
            row.StartDate = dto.StartDate;
            row.EndDate = dto.EndDate;
            row.Duration = dto.Duration;

            row.ParentId = dto.ParentId;
            row.IsMilestone = dto.IsMilestone;

            row.ResponsibleId = dto.ResponsibleId;

            row.LastModifiedField = dto.LastModifiedField;

        }

        private static GanttDto MapToDto(GanttTask row)
        {
            return new GanttDto
            {
                Id = row.Id,
                ParentId = row.ParentId,
                ProjectId = row.ProjectId,
                Name = row.Name,
                StartDate = row.StartDate,
                EndDate = row.EndDate,
                Duration = row.Duration,

                IsMilestone = row.IsMilestone,

                ResponsibleId = row.ResponsibleId,
                Capital = row.BudgetItemGanttTasks
                    .Where(x => x.BudgetItem.Category != BudgetCategory.Alteration)
                    .Sum(x => x.AmountAssigned),

                Expenses = row.BudgetItemGanttTasks
                    .Where(x => x.BudgetItem.Category == BudgetCategory.Alteration)
                    .Sum(x => x.AmountAssigned),
                LastModifiedField = row.LastModifiedField,
                Order = row.Order,
                Dependencies = row.Dependencies.Count == 0 ? new() : row.Dependencies.Select(x => new GanttDependencyDto()
                {
                    Id = x.Id,
                    PredecessorId = x.PredecessorId,
                    Lag = x.Lag,
                    Type = x.Type,
                    Order = x.Order,
                }).ToList(),
                Communications = row.Communications == null ? new() : row.Communications.Select(c => new CommunicationDto
                {
                    Id = c.Id,
                    Artifact = c.Artifact,
                    DaysOffsetOrFrequency = c.DaysOffsetOrFrequency,
                    Name = c.Name,
                    ProjectId = c.ProjectId,
                    Trigger = c.Trigger,
                    Type = c.Type,
                    Receivers = c.Receivers == null ? new() : c.Receivers.Select(r => new StakeHolderSimpleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Role = r.RoleInsideProject != null ? r.RoleInsideProject.Name : string.Empty

                    }).ToList()

                }).ToList()

            };

        }

        private void AssignOrderToChildren(GanttTask parent, List<GanttTask> all)
        {
            var children = all.Where(t => t.ParentId == parent.Id).OrderBy(t => t.Order).ToList();
            for (int i = 0; i < children.Count; i++)
                children[i].Order = i + 1;

            foreach (var child in children)
                AssignOrderToChildren(child, all);
        }

        private void ReassignOrderToFlatDfs(List<GanttTask> all)
        {
            var ordered = new List<GanttTask>();
            var rootTasks = all.Where(t => t.ParentId == null).OrderBy(t => t.Order).ToList();

            void Traverse(GanttTask task)
            {
                ordered.Add(task);
                var children = all
                    .Where(t => t.ParentId == task.Id)
                    .OrderBy(t => t.Order)
                    .ToList();
                foreach (var child in children)
                    Traverse(child);
            }

            foreach (var root in rootTasks)
                Traverse(root);

            for (int i = 0; i < ordered.Count; i++)
                ordered[i].Order = i + 1;
        }

        private static GanttDependency MapDependencyFromDto(GanttDependencyDto dto, Guid taskId)
        {
            return new GanttDependency
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                PredecessorId = dto.PredecessorId,
                Type = dto.Type,
                Lag = dto.Lag,
                Order = dto.Order,
            };
        }

        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ CreateGantt (ya sin HandleMutationAsync)
            app.MapPost("CreateGantt", async (CreateGantt dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid project." });

                var project = await _context.Projects.FindAsync(dto.ProjectId);
                if (project == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Project not found." });

                if (dto.Dependencies?.Count > 0)
                {
                    var validPredecessorIds = await _context.GanttTasks
                        .Where(t => t.ProjectId == dto.ProjectId && !t.IsDeleted && dto.Dependencies.Select(d => d.PredecessorId).Contains(t.Id))
                        .Select(t => t.Id)
                        .ToListAsync();

                    var invalidDeps = dto.Dependencies
                        .Where(d => d.PredecessorId != Guid.Empty && !validPredecessorIds.Contains(d.PredecessorId))
                        .ToList();

                    if (invalidDeps.Any())
                    {
                        var names = string.Join(", ", invalidDeps.Select(d => d.Predecessor?.Name ?? d.PredecessorId.ToString()));
                        return Results.Ok(new GeneralDto { Succeeded = false, Message = $"Invalid predecessors: {names}" });
                    }
                }

                var row = new GanttTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                    ParentId = dto.ParentId
                };

                MapFromDto(dto, row);
                await _context.GanttTasks.AddAsync(row);

                if (dto.Dependencies?.Count > 0)
                {
                    var deps = dto.Dependencies.Select(d => MapDependencyFromDto(d, row.Id)).ToList();
                    await _context.GanttDependencys.AddRangeAsync(deps);
                }

                var cacheKeyAll = $"{typeof(GetAllGanttTasks).Name}{dto.ProjectId}";
                var maxOrder = await getNextOrder.GetNextOrderAsync<GanttTask>(cacheKeyAll, dto.ProjectId);
                row.Order = maxOrder;
                project.LastModifiedOn = DateTime.UtcNow;

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(row.ProjectId, row.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Task created." });
            });

            // ✅ EditGantt (ya sin HandleMutationAsync)
            app.MapPost("EditGantt", async (EditGantt dto, IAppDbContext _context) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid project." });

                var row = await _context.GanttTasks.FindAsync(dto.Id);
                if (row == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Gantt task not found." });

                if (dto.Dependencies?.Count > 0)
                {
                    var validPredecessorIds = await _context.GanttTasks
                        .Where(t => t.ProjectId == dto.ProjectId && !t.IsDeleted && dto.Dependencies.Select(d => d.PredecessorId).Contains(t.Id))
                        .Select(t => t.Id)
                        .ToListAsync();

                    var invalidDeps = dto.Dependencies
                        .Where(d => d.PredecessorId != Guid.Empty && !validPredecessorIds.Contains(d.PredecessorId))
                        .ToList();

                    if (invalidDeps.Any())
                    {
                        var names = string.Join(", ", invalidDeps.Select(d => d.Predecessor?.Name ?? d.PredecessorId.ToString()));
                        return Results.Ok(new GeneralDto { Succeeded = false, Message = $"Invalid predecessors: {names}" });
                    }
                }

                MapFromDto(dto, row);

                var existingDeps = await _context.GanttDependencys.Where(d => d.TaskId == dto.Id).ToListAsync();
                _context.GanttDependencys.RemoveRange(existingDeps);

                if (dto.Dependencies?.Count > 0)
                {
                    var newDeps = dto.Dependencies.Select(d => MapDependencyFromDto(d, dto.Id)).ToList();
                    await _context.GanttDependencys.AddRangeAsync(newDeps);
                }

                var project = await _context.Projects.FindAsync(row.ProjectId);
                if (project != null)
                    project.LastModifiedOn = DateTime.UtcNow;

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(row.ProjectId, row.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Task updated." });
            });


            app.MapPost("DeleteGanttTask", async (DeleteGanttTask dto, IAppDbContext _context) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid project." });

                // 1. Cargamos la tarea con sus dependencias SALIENTES (Donde ella es la dueña)
                var task = await _context.GanttTasks
                    .Include(x => x.Dependencies)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted);

                if (task == null)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Gantt task not found." });

                // 2. Marcar borrado lógico de la tarea
                task.IsDeleted = true;

                // 3. Borrar dependencias SALIENTES (Esta tarea -> Otra)
                if (task.Dependencies != null)
                {
                    foreach (var dep in task.Dependencies)
                    {
                        dep.IsDeleted = true;
                    }
                }

                // 4. Borrar dependencias ENTRANTES (Otra -> Esta tarea)
                // CRÍTICO: Si no borras esto, otras tareas apuntarán a un fantasma y el cálculo fallará.
                var incomingDeps = await _context.GanttDependencys
                    .Where(d => d.PredecessorId == task.Id && !d.IsDeleted)
                    .ToListAsync();

                foreach (var inDep in incomingDeps)
                {
                    inDep.IsDeleted = true;
                }

                // 5. Gestión de Hijos: Los promovemos a la raíz (Orphan adoption)
                // Al poner ParentId = null, se convierten en tareas principales.
                var children = await _context.GanttTasks
                    .Where(x => x.ParentId == task.Id && !x.IsDeleted)
                    .ToListAsync();

                foreach (var child in children)
                {
                    child.ParentId = null;
                }

                // 6. Persistencia
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "No changes were saved." });

                // 7. Limpieza de Caché
                // Asumo que CacheKeys es un método local o helper disponible en tu Program.cs
                var keys = CacheKeys(task.ProjectId, task.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Task deleted successfully." });
            });
            // ✅ GetGanttTaskById (sin cambios)
            app.MapPost("GetGanttTaskById", async (GetGanttTaskById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetGanttTaskById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.GanttTasks
                        .AsSplitQuery()
                        .AsNoTracking()
                        .Include(x => x.Communications).ThenInclude(c => c.Receivers).ThenInclude(x => x.RoleInsideProject)
                        .Include(t => t.Dependencies)
                         .Include(t => t.BudgetItemGanttTasks).ThenInclude(b => b.BudgetItem)
                        .Where(x => x.Id == request.Id)
                        .FirstOrDefaultAsync();
                }, cacheKey);

                if (row == null)
                    return Results.Ok(new GeneralDto<GanttDto> { Succeeded = false, Message = "Gantt task not found." });

                var dto = MapToDto(row);


                return Results.Ok(new GeneralDto<GanttDto> { Succeeded = true, Data = dto });
            });

            // ✅ GetAllGanttTasks (sin cambios)
            app.MapPost("GetAllGanttTasks", async (GetAllGanttTasks request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllGanttTasks).Name}-{request.ProjectId}";
                var tasks = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.GanttTasks
                        .AsSplitQuery()
                        .AsNoTracking()
                         .Include(x => x.Communications).ThenInclude(x => x.Receivers).ThenInclude(x => x.RoleInsideProject)
                        .Include(t => t.Dependencies)
                        .Include(t => t.BudgetItemGanttTasks).ThenInclude(b => b.BudgetItem)
                        .Where(x => x.ProjectId == request.ProjectId)
                        .OrderBy(x => x.Order)
                        .ToListAsync();
                }, cacheKey);

                if (tasks == null || !tasks.Any())
                    return Results.Ok(new GeneralDto<List<GanttDto>> { Succeeded = true, Data = new() });

                var dtos = tasks.Select(MapToDto).ToList();




                return Results.Ok(new GeneralDto<List<GanttDto>> { Succeeded = true, Data = dtos });
            });

            // ✅ IndentGanttTaskRight (sin HandleMutationAsync)
            app.MapPost("IndentGanttTaskRight", async (IndentGanttTaskRight dto, IAppDbContext _context) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid project." });

                var task = await _context.GanttTasks.FindAsync(dto.Id);
                if (task == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Task not found." });

                bool targetExists = await _context.GanttTasks
                    .AnyAsync(t => t.Id == dto.TargetParentId
                                && t.ProjectId == dto.ProjectId
                                && !t.IsDeleted);
                if (!targetExists) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid target parent." });

                task.ParentId = dto.TargetParentId;

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(task.ProjectId, task.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Indent right succeeded." });
            });

            // ✅ IndentGanttTaskLeft (sin HandleMutationAsync)
            app.MapPost("IndentGanttTaskLeft", async (IndentGanttTaskLeft dto, IAppDbContext _context) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid project." });

                var task = await _context.GanttTasks.FindAsync(dto.Id);
                if (task == null || !task.ParentId.HasValue)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Task has no parent." });

                if (dto.NewParentId.HasValue)
                {
                    bool exists = await _context.GanttTasks
                        .AnyAsync(t => t.Id == dto.NewParentId && t.ProjectId == dto.ProjectId && !t.IsDeleted);
                    if (!exists) return Results.Ok(new GeneralDto { Succeeded = false, Message = "New parent not found." });
                }

                task.ParentId = dto.NewParentId;

                var all = await _context.GanttTasks
                    .Where(t => t.ProjectId == dto.ProjectId && !t.IsDeleted)
                    .ToListAsync();

                ReassignOrderToFlatDfs(all);
                _context.GanttTasks.UpdateRange(all);

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(task.ProjectId, task.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Indent left succeeded." });
            });

            // ✅ MoveGanttTaskUp (sin HandleMutationAsync)
            app.MapPost("MoveGanttTaskUp", async (MoveGanttTaskUp dto, IAppDbContext _context) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid project." });

                var task = await _context.GanttTasks.FindAsync(dto.Id);
                if (task == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Task not found." });

                var siblings = await _context.GanttTasks
                    .Where(t => t.ProjectId == dto.ProjectId
                             && t.ParentId == task.ParentId
                             && !t.IsDeleted)
                    .OrderBy(t => t.Order)
                    .ToListAsync();

                var index = siblings.FindIndex(t => t.Id == dto.Id);
                if (index <= 0) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Already at top." });

                var target = siblings[index - 1];
                (task.Order, target.Order) = (target.Order, task.Order);

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(task.ProjectId, task.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Move up succeeded." });
            });

            // ✅ MoveGanttTaskDown (sin HandleMutationAsync)
            app.MapPost("MoveGanttTaskDown", async (MoveGanttTaskDown dto, IAppDbContext _context) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Invalid project." });

                var task = await _context.GanttTasks.FindAsync(dto.Id);
                if (task == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Task not found." });

                var siblings = await _context.GanttTasks
                    .Where(t => t.ProjectId == dto.ProjectId
                             && t.ParentId == task.ParentId
                             && !t.IsDeleted)
                    .OrderBy(t => t.Order)
                    .ToListAsync();

                var index = siblings.FindIndex(t => t.Id == dto.Id);
                if (index == -1 || index >= siblings.Count - 1)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Already at bottom." });

                var target = siblings[index + 1];
                (task.Order, target.Order) = (target.Order, task.Order);

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                    return Results.Ok(new GeneralDto { Succeeded = false, Message = "Operation failed." });

                var keys = CacheKeys(task.ProjectId, task.Id);
                _context.InvalidateCache(keys);

                return Results.Ok(new GeneralDto { Succeeded = true, Message = "Move down succeeded." });
            });

            // ✅ ValidateGanttTaskName (sin cambios)
            // ✅ ValidateGanttTaskName (corregido, sin try/catch)
            app.MapPost("ValidateGanttTaskName", async (ValidateGanttTaskName dto, IAppDbContext _context) =>
            {
                if (dto.ProjectId == Guid.Empty)
                    return new GeneralDto<bool> { Succeeded = false, Message = "Invalid project." };

                try
                {
                    // ✅ Usar caché solo con Id y Name (más ligero)
                    var cacheKey = $"GetAllGanttTaskToValidateName-{dto.ProjectId}";
                    var tasks = await _context.GetOrAddCacheAsync(async () =>
                    {
                        return await _context.GanttTasks
                            .Where(x => x.ProjectId == dto.ProjectId && !x.IsDeleted)
                            .Select(x => new { x.Id, x.Name })
                            .ToListAsync();
                    }, cacheKey);

                    if (tasks == null)
                        return new GeneralDto<bool> { Succeeded = true, Data = true, Message = "Name is available." };

                    // ✅ Comparar con StringComparison.OrdinalIgnoreCase (tu caso de "alfonso" vs "Alfonso")
                    bool exists = tasks.Any(t =>
                        dto.Id == Guid.Empty
                            ? t.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)
                            : t.Id != dto.Id && t.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)
                    );

                    return new GeneralDto<bool>
                    {
                        Succeeded = true,
                        Data = !exists, // ✅ true = disponible, false = ya existe
                        Message = !exists ? "Name is available." : $"Name '{dto.Name}' already exists."
                    };
                }
                catch
                {
                    // ✅ Solo en caso extremo (ej: DB caída), responder sin exponer detalles
                    return new GeneralDto<bool>
                    {
                        Succeeded = true,
                        Data = false,
                        Message = "Validation unavailable. Please try again."
                    };
                }
            });
            app.MapPost("GetMonthlyExpendByProject", async (GetMonthlyExpendByProject request, IAppDbContext _context) =>
            {
                var project = await _context.Projects.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.ProjectId);
                if (project == null) return Results.NotFound();

                // 1. Catálogo completo de BudgetItems (Base para Budget USD)
                var allBudgetItems = await _context.BudgetItems
                    .Where(x => x.ProjectId == request.ProjectId && !x.IsDeleted)
                    .OrderBy(x => x.Category).ThenBy(x => x.Order)
                    .AsNoTracking().ToListAsync();

                // 2. Tareas del Gantt (Base para distribución mensual)
                var tasks = await _context.GanttTasks
                    .Include(t => t.BudgetItemGanttTasks)
                    .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted && t.EndDate.HasValue)
                    .AsNoTracking().ToListAsync();

                var columns = new List<string>();
                if (tasks.Any())
                {
                    var minDate = tasks.Min(t => t.StartDate ?? t.EndDate!.Value);
                    var maxDate = tasks.Max(t => t.EndDate!.Value);
                    for (var dt = new DateTime(minDate.Year, minDate.Month, 1); dt <= maxDate; dt = dt.AddMonths(1))
                        columns.Add(dt.ToString("MMM yyyy"));
                }
                else { columns.Add(DateTime.Now.ToString("MMM yyyy")); }

                var response = new MonthlyExpenditureResponse { Columns = columns };

                // --- 3. INICIALIZACIÓN Y CÁLCULO DE BUDGET USD (COLUMNA 3) ---
                var summaryCapital = new MonthlyExpenditureRow { BudgetName = "TOTAL CAPITAL", Nomenclatore = "CAP", IsSummary = true };
                var summaryAlterations = new MonthlyExpenditureRow { BudgetName = "TOTAL ALTERATIONS", Nomenclatore = "ALT", IsSummary = true };
                var taxRow = new MonthlyExpenditureRow { BudgetName = $"Taxes {project.PercentageTaxProductive}%", Nomenclatore = $"{BudgetCategory.Tax.GetLetter()}1", IsVirtual = true };
                var engRow = new MonthlyExpenditureRow { BudgetName = $"Engineering {project.PercentageEngineering}%", Nomenclatore = $"{BudgetCategory.Engineering.GetLetter()}1", IsVirtual = true };
                var contRow = new MonthlyExpenditureRow { BudgetName = $"Contingency {project.PercentageContingency}%", Nomenclatore = $"{BudgetCategory.Contingency.GetLetter()}1", IsVirtual = true };

                // Cálculo de Budget USD basado en los ítems del proyecto
                summaryCapital.OriginalBudget = allBudgetItems.Where(x => x.Category != BudgetCategory.Alteration).Sum(x => x.BudgetUSD);
                summaryAlterations.OriginalBudget = allBudgetItems.Where(x => x.Category == BudgetCategory.Alteration).Sum(x => x.BudgetUSD);

                if (!project.IsProductiveAsset)
                    taxRow.OriginalBudget = summaryCapital.OriginalBudget * (decimal)(project.PercentageTaxProductive / 100.0);

                decimal baseForProvisionsBudget = summaryCapital.OriginalBudget + taxRow.OriginalBudget;
                var totaltEng = project.PercentageEngineering + project.PercentageContingency;

                engRow.OriginalBudget = baseForProvisionsBudget * (decimal)(project.PercentageEngineering / (100.0 - totaltEng));
                contRow.OriginalBudget = baseForProvisionsBudget * (decimal)(project.PercentageContingency / (100.0 - totaltEng));

                // --- 4. PROCESAMIENTO DE FILAS REALES Y DISTRIBUCIÓN MENSUAL ---
                var itemRows = new List<MonthlyExpenditureRow>();
                foreach (var bi in allBudgetItems)
                {
                    var row = new MonthlyExpenditureRow { Id = bi.Id, BudgetName = bi.Name, Nomenclatore = bi.Nomenclatore, OriginalBudget = bi.BudgetUSD };

                    // Filtrar tareas que terminan en cada mes para este ítem
                    var assignments = tasks.SelectMany(t => t.BudgetItemGanttTasks
                                           .Where(a => a.BudgetItemId == bi.Id)
                                           .Select(a => new { Month = t.EndDate!.Value.ToString("MMM yyyy"), a.AmountAssigned }));

                    foreach (var ass in assignments)
                    {
                        row.AddAmount(ass.Month, ass.AmountAssigned);

                        // Acumular ejecución mensual en resúmenes
                        if (bi.Category != BudgetCategory.Alteration)
                            summaryCapital.AddAmount(ass.Month, ass.AmountAssigned);
                        else
                            summaryAlterations.AddAmount(ass.Month, ass.AmountAssigned);
                    }
                    itemRows.Add(row);
                }

                // --- 5. CÁLCULO MENSUAL DE FILAS VIRTUALES (CASCADA) ---
                foreach (var month in columns)
                {
                    decimal monthlyCap = summaryCapital.MonthlyValues.GetValueOrDefault(month, 0);
                    decimal monthlyTax = 0;

                    if (!project.IsProductiveAsset)
                    {
                        monthlyTax = monthlyCap * (decimal)(project.PercentageTaxProductive / 100.0);
                        taxRow.MonthlyValues[month] = monthlyTax;
                    }

                    decimal baseForProvisionMonth = monthlyCap + monthlyTax;
                    engRow.MonthlyValues[month] = baseForProvisionMonth * (decimal)(project.PercentageEngineering / (100.0 - totaltEng));
                    contRow.MonthlyValues[month] = baseForProvisionMonth * (decimal)(project.PercentageContingency / (100.0 - totaltEng));
                }

                // --- 6. ENSAMBLAJE FINAL ---
                var finalRows = new List<MonthlyExpenditureRow> { summaryCapital, summaryAlterations };
                finalRows.AddRange(itemRows);
                if (!project.IsProductiveAsset) finalRows.Add(taxRow);
                finalRows.Add(engRow);
                finalRows.Add(contRow);

                response.Rows = finalRows;
                return Results.Ok(new GeneralDto<MonthlyExpenditureResponse> { Succeeded = true, Data = response });
            });
            app.MapPost("GetBudgetItemAssignmentDetail", async (GetBudgetItemAssignmentDetail request, IAppDbContext _context) =>
            {
                var details = await _context.Set<BudgetItemGanttTask>()
                    .Include(x => x.GanttTask)
                    .Where(x => x.BudgetItemId == request.BudgetItemId && !x.GanttTask.IsDeleted)
                    .Select(x => new BudgetItemAssignmentDetailDto
                    {
                        TaskName = x.GanttTask.Name,
                        EndDate = x.GanttTask.EndDate,
                        AmountAssigned = x.AmountAssigned,
                        Progress = 0 // Asumiendo que Progress es 0-100
                    })
                    .OrderBy(x => x.EndDate)
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(new GeneralDto<List<BudgetItemAssignmentDetailDto>> { Succeeded = true, Data = details });
             
            });
        }
    }
}