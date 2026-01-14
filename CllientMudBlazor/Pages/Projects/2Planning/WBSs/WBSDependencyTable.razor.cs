using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.WBSs
{
    public partial class WBSDependencyTable
    {
        [Parameter] public List<GanttDependencyDto> Dependencies { get; set; } = new();
        [Parameter] public List<GanttDto> AllTasks { get; set; } = new();
        [Parameter] public Guid CurrentTaskId { get; set; }
        [Parameter] public EventCallback OnChanged { get; set; }

        private async Task Add()
        {
            var newDep = new GanttDependencyDto
            {
                Type = DependencyType.FinishToStart,
                Lag = "0d"
            };
            Dependencies.Add(newDep);
            RecalculateOrder();
            await OnChanged.InvokeAsync();
        }

        private async Task Remove(GanttDependencyDto dep)
        {
            Dependencies.Remove(dep);
            RecalculateOrder();
            await OnChanged.InvokeAsync();
        }

        private async Task MoveUp(GanttDependencyDto dep)
        {
            var index = Dependencies.IndexOf(dep);
            if (index <= 0) return;

            Dependencies.RemoveAt(index);
            Dependencies.Insert(index - 1, dep);

            RecalculateOrder();
            await OnChanged.InvokeAsync();
        }

        private async Task MoveDown(GanttDependencyDto dep)
        {
            var index = Dependencies.IndexOf(dep);
            if (index < 0 || index >= Dependencies.Count - 1) return;

            Dependencies.RemoveAt(index);
            Dependencies.Insert(index + 1, dep);

            RecalculateOrder();
            await OnChanged.InvokeAsync();
        }

        private void RecalculateOrder()
        {
            for (int i = 0; i < Dependencies.Count; i++)
            {
                Dependencies[i].Order = i + 1;
            }
        }

        private async Task HandlePredecessorChanged()
        {
            // Sincronizar IDs
            //foreach (var dep in Dependencies)
            //{
            //    if (dep.Predecessor != null)
            //        dep.PredecessorId = dep.Predecessor.Id;
            //}
            // Notificar para recalcular fechas en el Editor
            await OnChanged.InvokeAsync();
        }

        private string Truncate(string text, int maxLength) =>
            string.IsNullOrEmpty(text) ? "" :
            text.Length <= maxLength ? text : text[..(maxLength - 3)] + "...";

        private void OnDependencyChanged()
        {
            //var wasStartManual = Model.LastModifiedField == GanttField.StartDate;
            //var wasEndManual = Model.LastModifiedField == GanttField.EndDate;

            //// ✅ Incluir el modelo actual en el recálculo (importante en modo Create)
            //var contextDtos = Model.Id == Guid.Empty
            //    ? new List<GanttDto>(allTasks) { Model }  // agregar copia temporal
            //    : allTasks;

            //Model.LastModifiedField = null;
            //GanttCalculator.RecalculateDto(Model, contextDtos);
            //StateHasChanged();

            //if (wasStartManual) Model.LastModifiedField = GanttField.StartDate;
            //if (wasEndManual) Model.LastModifiedField = GanttField.EndDate;


        }
    }
}
