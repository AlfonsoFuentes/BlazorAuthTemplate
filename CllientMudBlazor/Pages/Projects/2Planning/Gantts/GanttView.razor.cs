using CllientMudBlazor.Pages.Projects._2Planning.BudgetItemGantt;
using CllientMudBlazor.Pages.Projects._2Planning.Communications;
using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Dtos.Projects._2._Plannings.Gantts;
using Shared.Dtos.Projects.Plannings.Gantts;
using Shared.Enums;

namespace CllientMudBlazor.Pages.Projects._2Planning.Gantts
{
    public partial class GanttView
    {
        [Parameter] public Guid ProjectId { get; set; }
        private List<GanttDto> tasks = new();

        protected override async Task OnParametersSetAsync()
        {
            if (ProjectId != Guid.Empty)
                await LoadTasks();
        }

        private async Task LoadTasks()
        {
            var request = new GetAllGanttTasks(ProjectId = ProjectId);
            var response = await HttpService.PostAsync<GetAllGanttTasks, GeneralDto<List<GanttDto>>>(request);
            if (response?.Succeeded == true)
            {
                tasks = response.Data.RecalculateAllTasks() ?? new();
            }

        }

        private async Task OpenCreateDialog()
        {
            CreateGantt dto = new CreateGantt()
            {
                ProjectId = ProjectId
            };
            var parameters = new DialogParameters<GanttTaskDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<GanttTaskDialog>($"Add Task", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await LoadTasks();
                StateHasChanged();
            }
        }

        private async Task OpenEditDialog()
        {

            if (SelectedTask == null) return;
            GanttDto task = SelectedTask;
            EditGantt dto = new EditGantt()
            {
                Id = task.Id,
                ProjectId = ProjectId
            };

            var parameters = new DialogParameters<GanttTaskDialog>
            {
                { x => x.Model, dto},
            };
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium };
            var dialog = await DialogService.ShowAsync<GanttTaskDialog>("Edit Task", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await LoadTasks();
                StateHasChanged();
            }
        }



        private async Task DeleteTask()
        {

            if (SelectedTask == null) return;
            GanttDto task = SelectedTask;
            var confirm = await DialogService.ShowMessageBox(
                "Confirm Delete",
                $"Are you sure you want to delete '{task.Name}'?",
                yesText: "Delete", noText: "Cancel");

            if (confirm != true) return;

            var request = new DeleteGanttTask(task.Id, ProjectId);
            var response = await HttpService.PostAsync<DeleteGanttTask, GeneralDto>(request);
            if (response?.Succeeded == true)
            {
                await LoadTasks();          // recarga tasks
                RefreshSelectedTask();      // ✅ re-enlaza SelectedTask
                StateHasChanged();          // fuerza re-render + reevaluación de CanIndentRight()
            }
        }
        // 🔹 Métodos de acción
        private async Task IndentRight()
        {
            if (SelectedTask == null) return;

            var target = GetIndentRightTarget(SelectedTask);
            if (target == null) return;

            var request = new IndentGanttTaskRight(
                Id: SelectedTask.Id,
                ProjectId: ProjectId,
                TargetParentId: target.Id // ✅ explícito
            );

            var response = await HttpService.PostAsync<IndentGanttTaskRight, GeneralDto>(request);
            if (response?.Succeeded == true)
            {
                await LoadTasks();          // recarga tasks
                RefreshSelectedTask();      // ✅ re-enlaza SelectedTask
                StateHasChanged();          // fuerza re-render + reevaluación de CanIndentRight()
            }
        }

        private async Task IndentLeft()
        {
            if (SelectedTask == null || !SelectedTask.ParentId.HasValue) return;

            // Obtener el abuelo (puede ser null → raíz)
            var parent = tasks.FirstOrDefault(t => t.Id == SelectedTask.ParentId);
            var newParentId = parent?.ParentId; // null si el padre era raíz

            var request = new IndentGanttTaskLeft(
                Id: SelectedTask.Id,
                ProjectId: ProjectId,
                NewParentId: newParentId
            );

            var response = await HttpService.PostAsync<IndentGanttTaskLeft, GeneralDto>(request);
            if (response?.Succeeded == true)
            {
                await LoadTasks();          // recarga tasks
                RefreshSelectedTask();      // ✅ re-enlaza SelectedTask
                StateHasChanged();          // fuerza re-render + reevaluación de CanIndentRight()
            }
        }
        private void RefreshSelectedTask()
        {
            if (SelectedTask != null)
            {
                SelectedTask = tasks.FirstOrDefault(t => t.Id == SelectedTask.Id);
            }
        }
        // 🔹 Reglas de habilitación (mejoradas)


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
        private GanttDto? SelectedTask = null!;



        void OnRowClick(TableRowClickEventArgs<GanttDto> arg)
        {
            if (arg.Item == null)
            {
                SelectedTask = null;
                return;
            }
            if (SelectedTask == null)
            {
                SelectedTask = arg.Item;
                return;
            }
            if (SelectedTask.Id == arg.Item.Id)
            {
                SelectedTask = null;
                return;
            }
            SelectedTask = arg.Item;

            StateHasChanged();
        }
        // 🔹 Ayudante: evitar ciclos (task → ... → task)



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

        private async Task MoveTaskUp()
        {
            if (SelectedTask == null) return;

            var request = new MoveGanttTaskUp(SelectedTask.Id, ProjectId);
            var response = await HttpService.PostAsync<MoveGanttTaskUp, GeneralDto>(request);
            if (response?.Succeeded == true)
            {
                await LoadTasks();
                RefreshSelectedTask();
                StateHasChanged();
            }
        }

        private async Task MoveTaskDown()
        {
            if (SelectedTask == null) return;

            var request = new MoveGanttTaskDown(SelectedTask.Id, ProjectId);
            var response = await HttpService.PostAsync<MoveGanttTaskDown, GeneralDto>(request);
            if (response?.Succeeded == true)
            {
                await LoadTasks();
                RefreshSelectedTask();
                StateHasChanged();
            }
        }
        private bool HasCommunications(GanttDto? task)
        {
            if (task == null || task.Communications == null) return false;
            return task.Communications.Any();
        }
        private async Task ManageCommunication()
        {
            if (SelectedTask == null) return;

            // CASO A: No hay plan -> Abrir formulario CREAR directo
            if (!HasCommunications(SelectedTask))
            {
                await OpenCreateCommDialog(SelectedTask);
            }
            // CASO B: Hay plan -> Abrir LISTADO para gestionar
            else
            {
                await OpenListCommDialog(SelectedTask);
            }
        }

        // 1. Abrir formulario de creación
        private async Task OpenCreateCommDialog(GanttDto task)
        {
            var newComm = new CreateCommunication
            {
                ProjectId = ProjectId, // Asegúrate de tener esta variable disponible
                SelectedGanttTask = task,
                Trigger = CommunicationTrigger.TaskEnd // Default sugerido
            };
            var parameters = new DialogParameters<CommunicationDialog>
            {
                { x => x.Model, newComm},
                  { x => x.FixedTask, task},
            };
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<CommunicationDialog>("Plan Communication", parameters, options);
            var result = await dialog.Result;

            if (!result!.Canceled)
            {
                // IMPORTANTE: Recargar datos para que el botón se ponga azul y se actualice la lista
                await LoadTasks();
            }
        }

        // 2. Abrir listado de gestión
        private async Task OpenListCommDialog(GanttDto task)
        {
            var parameters = new DialogParameters<CommunicationListDialog>
            {
                { x => x.TaskCommunications, task.Communications},
                  { x => x.TaskContext, task},
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small };
            var dialog = await DialogService.ShowAsync<CommunicationListDialog>($"Communications for: {task.Name}", parameters, options);
            var result = await dialog.Result;

            // Si el diálogo devuelve "Reload", recargamos todo
            if (!result!.Canceled)
            {
                await LoadTasks();
            }
        }
        private async Task ManageCommunicationFromRow(GanttDto task)
        {
            // Opcional: Seleccionamos la fila visualmente para que se entienda el contexto
            SelectedTask = task;

            // Llamamos a la misma lógica que ya creamos
            await OpenListCommDialog(task);
        }
    
     [Parameter]
        public bool DisableAddEdit { get; set; } = false;
        // 🔹 Filtro gestionado aquí (como querías)

        [Parameter]
        public EventCallback HideTable { get; set; }
        async Task OnHideTable()
        {
            if (HideTable.HasDelegate) await HideTable.InvokeAsync();
        }


        private void ToggleExpand(GanttDto dto) => dto.IsExpanded = !dto.IsExpanded;

        private MudTable<GanttDto> mudTable = null!;
        private string SelectedRowClassFunc(GanttDto element, int rowNumber)
        {
            var classes = new List<string>();

            // ✅ Si tiene hijos → negrita
            if (element.Children?.Count > 0)
            {
                classes.Add("gantt-task-parent");
            }

            // ✅ Si está seleccionada → resaltar
            if (mudTable?.SelectedItem != null && mudTable.SelectedItem.Id == element.Id)
            {
                classes.Add("gantt-selected");
            }

            return string.Join(" ", classes);
        }
        private bool IsVisible(GanttDto task)
        {
            var current = task;
            while (current.ParentId.HasValue)
            {
                var parent = tasks.FirstOrDefault(t => t.Id == current.ParentId);
                if (parent == null || !parent.IsExpanded)
                    return false;
                current = parent;
            }
            return true;
        }
        private async Task OpenBudgetDialogFromRow(GanttDto task)
        {
            // 1. Sincronizamos la selección visual
            SelectedTask = task;

            // 2. Definimos los parámetros para el diálogo
            var parameters = new DialogParameters<GanttBudgetDialog>
    {
        { x => x.GanttTaskId, task.Id },
        { x => x.ProjectId, ProjectId },
        { x => x.TaskName, task.Name },
        { x => x.DisableAddEdit, DisableAddEdit }
    };

            // 3. Configuramos las opciones de visualización
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                BackdropClick = false // Evita que se cierre por error si el usuario hace clic fuera
            };

            // 4. Mostramos el diálogo
            var dialog = await DialogService.ShowAsync<GanttBudgetDialog>("Budget Management", parameters, options);
            var result = await dialog.Result;

            // 5. Si el usuario guardó cambios o eliminó algo, recargamos para ver los nuevos totales
            if (!result!.Canceled)
            {
                await LoadTasks(); // Recarga desde el servidor para traer Capital y Expenses actualizados
                StateHasChanged(); // Refresca la UI
            }
        }
    }
}
