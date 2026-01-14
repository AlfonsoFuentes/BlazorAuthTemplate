using CllientMudBlazor.Pages.Projects._2Planning.BudgetItemGantt;
using CllientMudBlazor.Pages.Projects._2Planning.Communications;
using CllientMudBlazor.Pages.Projects._2Planning.Gantts;
using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Dtos.Projects.Plannings.Gantts;
using Shared.Enums;

namespace CllientMudBlazor.Pages.Projects._2Planning.WBSs
{
    public partial class WBSCardNodeV2
    {




        [Parameter] public int Level { get; set; }
        [Parameter] public GanttDto Task { get; set; } = new();
        [Parameter] public List<GanttDto> AllTasks { get; set; } = new();
        [Parameter] public bool DisabledAddEdit { get; set; } = false;
        [Parameter, EditorRequired] public EventCallback GetAll { get; set; }
        // Eventos de Gestión de Tareas
        [Parameter] public EventCallback<Guid?> OnAdd { get; set; }

        [Parameter] public EventCallback<GanttDto> OnDelete { get; set; }

        // Eventos de Movimiento y Jerarquía
        [Parameter] public EventCallback<GanttDto> OnMoveUp { get; set; }
        [Parameter] public EventCallback<GanttDto> OnMoveDown { get; set; }
        [Parameter] public EventCallback<GanttDto> OnIndentLeft { get; set; }
        [Parameter] public EventCallback<GanttDto> OnIndentRight { get; set; }
        protected override void OnInitialized()
        {
            if (Task.Id == Guid.Empty)
            {
                PrepareCreateMode();
            }
        }
        private bool isCreating = false;
        private CreateGantt createModel = null!; // El DTO que tu endpoint espera

        private void PrepareCreateMode()
        {
            isCreating = true;
            isEditing = false; // Activamos el panel de edición

            // Inicializamos el objeto exacto que espera tu endpoint de creación
            createModel = new CreateGantt
            {
                ProjectId = Task.ProjectId,
                ParentId = Task.ParentId,
                Name = "",
                StartDate = Task.StartDate,
                EndDate = Task.EndDate,
                Duration = Task.Duration,
                Order = Task.Order
            };
        }
        private bool isEditing = false;
        private EditGantt editModel = null!;

        private void EnableEdit()
        {
            // Clonación para el EditForm
            editModel = new EditGantt
            {
                Id = Task.Id,
                ProjectId = Task.ProjectId,
                Name = Task.Name,
                StartDate = Task.StartDate,
                EndDate = Task.EndDate,
                Duration = Task.Duration,
                ParentId = Task.ParentId,
                Dependencies = Task.Dependencies.Select(d => new GanttDependencyDto
                {
               
                    Predecessor = d.Predecessor, // Re-vinculado por el Calculator V2
                    Type = d.Type,
                    Lag = d.Lag,
                    Order = d.Order
                }).ToList()
            };
            isEditing = true;
        }

        private async Task HandleValidSubmit()
        {
            GeneralDto result = null!;

            if (isCreating)
            {
                // Enviar explícitamente el DTO de creación
                result = await HttpService.PostAsync<CreateGantt, GeneralDto>(createModel);
            }
            else if (isEditing)
            {
                // Enviar explícitamente el DTO de edición
                result = await HttpService.PostAsync<EditGantt, GeneralDto>(editModel);
            }

            if (result != null && result.Succeeded)
            {
                isEditing = false;
                isCreating = false;
                await GetAll.InvokeAsync();
            }
        }

        private async Task Cancel()
        {
            if (isCreating)
            {
                // Notificar al padre que elimine la tarea temporal de la lista 'tasks'
                // Podemos re-usar el LoadTasks o un evento específico
                await GetAll.InvokeAsync();
            }
            isEditing = false;
            isCreating = false;
        }
        private void ToggleExpand() => Task.IsExpanded = !Task.IsExpanded;
        private string GetNodeClass() => Task.Children?.Any() == true ? "wbs-node wbs-parent" : "wbs-node wbs-leaf";

        // Wrappers para invocar los callbacks
        private async Task Add(Guid id) => await OnAdd.InvokeAsync(id);
        private async Task Delete(GanttDto task) => await OnDelete.InvokeAsync(task);
        private async Task MoveUp(GanttDto task) => await OnMoveUp.InvokeAsync(task);
        private async Task MoveDown(GanttDto task) => await OnMoveDown.InvokeAsync(task);
        private async Task IndentLeft(GanttDto task) => await OnIndentLeft.InvokeAsync(task);
        private async Task IndentRight(GanttDto task) => await OnIndentRight.InvokeAsync(task);
        private bool HasCommunications => Task.Communications != null && Task.Communications.Any();

        private async Task ManageCommunication()
        {
            // CASO A: No hay plan -> Abrir formulario CREAR directo
            if (!HasCommunications)
            {
                await OpenCreateCommDialog();
            }
            // CASO B: Hay plan -> Abrir LISTADO para gestionar
            else
            {
                await OpenListCommDialog();
            }
        }

        private async Task OpenCreateCommDialog()
        {
            var newComm = new CreateCommunication
            {
                ProjectId = Task.ProjectId,
                SelectedGanttTask = Task,
                Trigger = CommunicationTrigger.TaskEnd
            };

            var parameters = new DialogParameters
            {
                ["Model"] = newComm,
                ["FixedTask"] = Task
            };

            var dialog = await DialogService.ShowAsync<CommunicationDialog>("Plan Communication", parameters);
            var result = await dialog.Result;

            if (!result!.Canceled)
            {
                // Importante: Refrescar todo el árbol WBS llamando al padre
                if (GetAll.HasDelegate) await GetAll.InvokeAsync();
            }
        }
        private async Task OpenListCommDialog()
        {
            var parameters = new DialogParameters
            {
                ["TaskCommunications"] = Task.Communications,
                ["TaskContext"] = Task
            };

            var dialog =await DialogService.ShowAsync<CommunicationListDialog>($"Communications: {Task.Name}", parameters);
            var result = await dialog.Result;

            // Si hubo cambios (crear/borrar/editar) en el listado, recargamos el árbol
            if (!result!.Canceled)
            {
                if (GetAll.HasDelegate) await GetAll.InvokeAsync();
            }
        }
        // En el partial class de WBSCardNodeV2
        private async Task OpenBudgetDialog()
        {
            var parameters = new DialogParameters<GanttBudgetDialog>
    {
        { x => x.GanttTaskId, Task.Id },
        { x => x.ProjectId, Task.ProjectId },
        { x => x.TaskName, Task.Name },
        { x => x.DisableAddEdit, false } // O pasar el parámetro del componente
    };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            var dialog = await DialogService.ShowAsync<GanttBudgetDialog>("Budget Management", parameters, options);
            var result = await dialog.Result;

            if (!result!.Canceled)
            {
                // Importante: Llamar al EventCallback que recarga las tareas en el padre (WBSView)
                await GetAll.InvokeAsync();
            }
        }
    }
}
