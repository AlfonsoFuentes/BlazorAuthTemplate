using CllientMudBlazor.Pages.Projects._2Planning.Gantts;
using Microsoft.JSInterop;
using Shared.Dtos.Projects;
using Shared.Dtos.Projects._2._Plannings.Gantts;
using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.WBSs
{
    public partial class WBS
    {
        [Parameter] public Guid ProjectId { get; set; }
        private List<GanttDto> tasks = new();
        private List<GanttDto> roots = new();
        protected override async Task OnInitializedAsync()
        {
            var result = await HttpService.PostAsync<GetInitialProjectDate, GeneralDto<DateTime>>(new GetInitialProjectDate(ProjectId));
            if (result.Succeeded)
            {
                InitialProjectDate = result.Data;
            }
        }
        protected override async Task OnParametersSetAsync()
        {
            if (ProjectId != Guid.Empty)
                await LoadTasks();
        }
        DateTime InitialProjectDate;
        private async Task LoadTasks()
        {
            var request = new GetAllGanttTasks(ProjectId);
            var response = await HttpService.PostAsync<GetAllGanttTasks, GeneralDto<List<GanttDto>>>(request);
            if (response?.Succeeded == true)
            {
                tasks = response.Data.RecalculateAllTasks() ?? new();
                // ✅ Asegurar que Children esté lleno (tu RecalculateDto ya lo hace)
                roots = tasks.Where(t => t.ParentId == null).OrderBy(t => t.Order).ToList();
            }
           

        }
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;// Asegúrate de inyectarlo
        private async Task OpenCreateRootTask()
        {
            // Calculamos el ID visual correlativo (el máximo actual + 1)
            int nextIdNumber = tasks.Any() ? tasks.Max(t => t.IdNumber) + 1 : 1;

            // Calculamos el siguiente WBS para nivel raíz
            // Si la última raíz es "4", la nueva es "5"
            int nextRootOrder = tasks.Where(t => t.ParentId == null).Any()
                                ? tasks.Where(t => t.ParentId == null).Max(x => x.Order) + 1
                                : 1;

            var tempTask = new GanttDto
            {
                Id = Guid.Empty,
                IdNumber = nextIdNumber,
                WbsCode = nextRootOrder.ToString(), // WBS temporal
                ProjectId = ProjectId,
                ParentId = null,
                Name = "",
                StartDate = InitialProjectDate,
                EndDate = InitialProjectDate.AddDays(1),
                Duration = "1d",
                Order = nextRootOrder,
                Dependencies = new()
            };

            tasks.Add(tempTask);
            roots = tasks.Where(t => t.ParentId == null).OrderBy(t => t.Order).ToList();
           
            StateHasChanged();
            await Task.Delay(100);
            await JSRuntime.InvokeVoidAsync("scrollToElement", $"task-{tempTask.IdNumber}");
        }

        private async Task OpenCreateDialog(Guid? parentId)
        {
            var dto = new CreateGantt { ProjectId = ProjectId, ParentId = parentId };
            var parameters = new DialogParameters<GanttTaskDialog>
        {
            { x => x.Model, dto }
        };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, CloseOnEscapeKey = true };

            var dialog = await DialogService.ShowAsync<GanttTaskDialog>("Add Task", parameters, options);
            var result = await dialog.Result;
            if (result is not null)
                await LoadTasks(); // recargar
        }


        private async Task DeleteTask(GanttDto task)
        {
            var confirm = await DialogService.ShowMessageBoxAsync(
                "Confirm Delete",
                $"Are you sure you want to delete '{task.Name}' and its subtasks?",
                yesText: "Delete", noText: "Cancel");

            if (confirm != true) return;

            var request = new DeleteGanttTask(task.Id, ProjectId);
            var response = await HttpService.PostAsync<DeleteGanttTask, GeneralDto>(request);
            if (response?.Succeeded == true)
                await LoadTasks();
        }
        private async Task MoveTaskUp(GanttDto task)
        {
            var request = new MoveGanttTaskUp(task.Id, ProjectId);
            var response = await HttpService.PostAsync<MoveGanttTaskUp, GeneralDto>(request);
            if (response.Succeeded)
                await LoadTasks();
        }

        private async Task MoveTaskDown(GanttDto task)
        {
            var request = new MoveGanttTaskDown(task.Id, ProjectId);
            var response = await HttpService.PostAsync<MoveGanttTaskDown, GeneralDto>(request);
            if (response.Succeeded)
                await LoadTasks();
        }

        private async Task IndentTaskRight(GanttDto task)
        {
            var target = GetIndentRightTarget(task);
            if (target == null) return;

            var request = new IndentGanttTaskRight(task.Id, ProjectId, target.Id);
            var response = await HttpService.PostAsync<IndentGanttTaskRight, GeneralDto>(request);
            if (response.Succeeded)
                await LoadTasks();
        }

        private async Task IndentTaskLeft(GanttDto task)
        {
            var parent = tasks.FirstOrDefault(t => t.Id == task.ParentId);
            var newParentId = parent?.ParentId;

            var request = new IndentGanttTaskLeft(task.Id, ProjectId, newParentId);
            var response = await HttpService.PostAsync<IndentGanttTaskLeft, GeneralDto>(request);
            if (response.Succeeded)
                await LoadTasks();
        }
        private GanttDto? GetIndentRightTarget(GanttDto task)
        {
            var index = tasks.FindIndex(t => t.Id == task.Id);
            if (index <= 0) return null;

            int taskLevel = task.WbsCode.Count(c => c == '.');

            // 🔹 Buscar hacia atrás la PRIMERA tarea con el MISMO nivel
            for (int i = index - 1; i >= 0; i--)
            {
                var candidate = tasks[i];
                int candLevel = candidate.WbsCode.Count(c => c == '.');

                if (candLevel == taskLevel)
                {
                    // ✅ Evitar ciclos (aunque improbable en mismo nivel)
                    if (!IsAncestor(task, candidate))
                        return candidate;
                }
            }

            return null;
        }
        private bool IsAncestor(GanttDto potentialAncestor, GanttDto child)
        {
            var current = child;
            while (current.ParentId.HasValue)
            {
                if (current.ParentId == potentialAncestor.Id) return true;
                current = tasks.FirstOrDefault(t => t.Id == current.ParentId);
                if (current == null) break;
            }
            return false;
        }
        // Método que se llama desde el componente hijo
        private async Task OpenCreateChildInline(Guid parentId)
        {
            var parent = tasks.FirstOrDefault(t => t.Id == parentId);
            if (parent == null) return;

            // 1. Expandir al padre
            parent.IsExpanded = true;

            // 2. Crear la tarea temporal (ficticia)
            // Usamos Guid.Empty para marcarla como nueva, pero IdNumber temporal
            var tempChild = new GanttDto
            {
                Id = Guid.Empty,
                ParentId = parentId,
                ProjectId = ProjectId,
                Name = "",
                StartDate = parent.StartDate,
                EndDate = parent.StartDate?.AddDays(1),
                Order = (tasks.Where(t => t.ParentId == parentId).Max(x => (int?)x.Order) ?? 0) + 1
            };

            // 3. Insertar en la lista global
            tasks.Add(tempChild);

            // 4. RECALCULAR TODO EL ÁRBOL
            // Aquí es donde tu lógica actual de cálculo de WBS e IdNumber entra en acción
            // para que la tarea ficticia reciba sus números oficiales.
            RecalculateWbsAndIdNumbers();

            // 5. Refrescar la UI para que los IDs existan en el DOM
            StateHasChanged();

            // 6. Ahora que el IdNumber es correcto y único, hacemos el scroll
            await Task.Delay(200);
            await JSRuntime.InvokeVoidAsync("scrollToElement", $"task-{tempChild.IdNumber}");
        }
        private void RecalculateWbsAndIdNumbers()
        {
            // 1. Llamamos al potente método del Calculator que ya tienes.
            // Este método: ordena, asigna WBS (1.1, 1.2), asigna IdNumbers y recalcula fechas.
            tasks = GanttCalculatorV3.RecalculateAllTasks(tasks);

            // 2. IMPORTANTE: Refrescamos la lista de "roots" (raíces)
            // porque el ordenamiento pudo haber cambiado.
            roots = tasks.Where(t => t.ParentId == null).OrderBy(t => t.Order).ToList();
        }
    }
}

