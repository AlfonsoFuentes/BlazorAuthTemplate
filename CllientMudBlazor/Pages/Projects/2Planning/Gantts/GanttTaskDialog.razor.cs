using Shared.Dtos.Projects;
using Shared.Dtos.Projects._2._Plannings.Gantts;
using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.Gantts
{


    public partial class GanttTaskDialog
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
        [Parameter] public GanttDto Model { get; set; } = new();


        private List<GanttDto> allTasks = new();
        private bool IsCalculating = false;
        private char? lastDurationUnit = 'd';


        protected override async Task OnInitializedAsync()
        {
            var response = await HttpService.PostAsync<GetAllGanttTasks, GeneralDto<List<GanttDto>>>(new(Model.ProjectId));
            allTasks = response.Succeeded ? response.Data.RecalculateAllTasks() ?? new() : new();

            // ✅ Eliminar la tarea actual de allTasks (evita autodependencia en UI)
            if (Model.Id != Guid.Empty)
            {
                allTasks = allTasks.Where(t => t.Id != Model.Id).ToList();
            }

            if (Model.Id != Guid.Empty)
            {
                // ✅ Modo Editar: cargar datos reales (incluyendo dependencias)
                var resultById = await HttpService.PostAsync<GetGanttTaskById, GeneralDto<EditGantt>>(new GetGanttTaskById(Model.Id));
                if (resultById.Succeeded)
                {
                    Model = resultById.Data;
                    // ✅ Asegurar que las dependencias tengan Predecessor cargado
                    if (Model.Dependencies != null)
                    {
                        foreach (var dep in Model.Dependencies)
                        {
                            dep.Predecessor = allTasks.FirstOrDefault(t => t.Id == dep.PredecessorId);
                            // ✅ Manejo defensivo: si no se encuentra, evitar null

                        }
                    }
                }
            }
            else
            {
                // ✅ Modo Crear: inicializar fechas, pero NO sobreescribir Duration si ya vino seteado
                var result = await HttpService.PostAsync<GetInitialProjectDate, GeneralDto<DateTime>>(new(Model.ProjectId));
                if (result.Succeeded && Model.StartDate == null)
                {
                    Model.StartDate = result.Data;

                    // ✅ Solo recalcular si no hay Duration explícita
                    if (string.IsNullOrWhiteSpace(Model.Duration) || Model.Duration == "1d")
                    {
                        OnDurationChanged(); // → recalcula Duration desde StartDate
                    }
                }
            }


        }

        private void AddDependency()
        {
            var newDep = new GanttDependencyDto() { Type = DependencyType.FinishToStart };
            Model.Dependencies.Add(newDep);
            RecalculateOrder();
            // ✅ Iniciar edición inmediata en la nueva fila


        }

        private void RemoveDependency(GanttDependencyDto dep)
        {
            Model.Dependencies.Remove(dep);

        }
        private void MoveDependencyUp(GanttDependencyDto dep)
        {
            var index = Model.Dependencies.IndexOf(dep);
            if (index <= 0) return;

            Model.Dependencies.RemoveAt(index);
            Model.Dependencies.Insert(index - 1, dep);
            RecalculateOrder();
            OnDependencyChanged(); // ✅ recalcular fechas tras reordenar
        }

        private void MoveDependencyDown(GanttDependencyDto dep)
        {
            var index = Model.Dependencies.IndexOf(dep);
            if (index < 0 || index >= Model.Dependencies.Count - 1) return;

            Model.Dependencies.RemoveAt(index);
            Model.Dependencies.Insert(index + 1, dep);
            RecalculateOrder();
            OnDependencyChanged(); // ✅ recalcular fechas tras reordenar
        }

        private void RecalculateOrder()
        {
            for (int i = 0; i < Model.Dependencies.Count; i++)
            {
                Model.Dependencies[i].Order = i + 1;
            }
        }
        private async Task Submit()
        {
            // ✅ Ya validado reactivamente — solo guardar
            var result = await HttpService.PostAsync<GanttDto, GeneralDto>(Model);
            if (result.Succeeded) MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();

        // ✅ Manejo de cambios con validación
        private void OnStartDateChanged()
        {
            if (IsCalculating) return;
            Model.LastModifiedField = GanttField.StartDate;
            Recalculate();
        }

        private void OnEndDateChanged()
        {
            if (IsCalculating) return;
            Model.LastModifiedField = GanttField.EndDate;
            Recalculate();
        }

        private void OnDurationChanged()
        {
            if (IsCalculating) return;
            var raw = Model.Duration?.Trim() ?? string.Empty;
            var parsed = DurationParser.TryParseDetailed(raw);
            Model.LastModifiedField = GanttField.Duration;
            if (parsed.HasValue)
            {
                var (amount, unitChar, hadUnit) = parsed.Value;
                if (hadUnit)
                {
                    lastDurationUnit = unitChar;
                    Model.Duration = $"{amount}{unitChar}";
                }
                else if (lastDurationUnit.HasValue)
                {
                    Model.Duration = $"{amount}{lastDurationUnit}";
                }
            }


            Recalculate();
        }

        private void Recalculate()
        {
            GanttCalculatorV3.RecalculateDto(Model, allTasks);

            StateHasChanged();
        }

        // ✅ Validación reactiva (sin Snackbar, solo estado UI)


        // ✅ Nuevo: Recálculo INTELIGENTE tras cambios en dependencias
        private void OnDependencyChanged()
        {
            var wasStartManual = Model.LastModifiedField == GanttField.StartDate;
            var wasEndManual = Model.LastModifiedField == GanttField.EndDate;

            // ✅ Incluir el modelo actual en el recálculo (importante en modo Create)
            var contextDtos = Model.Id == Guid.Empty
                ? new List<GanttDto>(allTasks) { Model }  // agregar copia temporal
                : allTasks;

            Model.LastModifiedField = null;
            GanttCalculatorV3.RecalculateDto(Model, contextDtos);
            StateHasChanged();

            if (wasStartManual) Model.LastModifiedField = GanttField.StartDate;
            if (wasEndManual) Model.LastModifiedField = GanttField.EndDate;


        }



    }

}

