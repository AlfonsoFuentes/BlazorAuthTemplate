using Blazored.FluentValidation;
using Shared.Dtos.Projects._2._Plannings.Gantts;
using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.WBSs
{
    public partial class WBSCardEditor
    {
        [Parameter] public GanttDto Model { get; set; } = null!;
        [Parameter] public List<GanttDto> AllTasks { get; set; } = null!;
        [Parameter] public EventCallback OnSave { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        private void OnStartDateChanged() => Recalculate(GanttField.StartDate);
        private void OnEndDateChanged() => Recalculate(GanttField.EndDate);
        private void OnDurationChanged() => Recalculate(GanttField.Duration);
        // En WBSCardEditor.razor.cs (siguiendo tu lógica del TaskDialog)
        private void OnDependencyChanged()
        {
            var wasStartManual = Model.LastModifiedField == GanttField.StartDate;
            var wasEndManual = Model.LastModifiedField == GanttField.EndDate;

            // Si es una tarea nueva (ID vacío), incluimos el modelo en una lista temporal
            var contextDtos = Model.Id == Guid.Empty
                ? new List<GanttDto>(AllTasks) { Model }
                : AllTasks;

            Model.LastModifiedField = null;

            // Ejecutar el motor de cálculo
            GanttCalculatorV3.RecalculateDto(Model, contextDtos);

            // Restaurar estado de edición manual si existía
            if (wasStartManual) Model.LastModifiedField = GanttField.StartDate;
            if (wasEndManual) Model.LastModifiedField = GanttField.EndDate;

            StateHasChanged();
        }

        private void Recalculate(GanttField field)
        {
            Model.LastModifiedField = field;
            var start = Model.StartDate;
            var end = Model.EndDate;
            var dur = Model.Duration;

            GanttCalculatorV3.Recalculate_V1_Math(ref start, ref end, ref dur, field);

            Model.StartDate = start;
            Model.EndDate = end;
            Model.Duration = dur;
        }
        private FluentValidationValidator? _validator;

        private bool IsValid => _validator?.Validate(options => options.IncludeAllRuleSets()) ?? false;
        private async Task Cancel()
        {
            if (OnCancel.HasDelegate) await OnCancel.InvokeAsync();
         
        }
    }

}
