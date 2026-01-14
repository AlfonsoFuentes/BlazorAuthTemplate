using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.WBSs
{
    public partial class WBSCardNode
    {
     
        [Parameter] public int Level { get; set; }
        [Parameter] public GanttDto Task { get; set; } = new();

        [Parameter] public EventCallback<Guid?> OnAdd { get; set; }
        [Parameter] public EventCallback<GanttDto> OnEdit { get; set; }
        [Parameter] public EventCallback<GanttDto> OnDelete { get; set; }
        [Parameter] public EventCallback<GanttDto> OnMoveUp { get; set; }
        [Parameter] public EventCallback<GanttDto> OnMoveDown { get; set; }
        [Parameter] public EventCallback<GanttDto> OnIndentLeft { get; set; }
        [Parameter] public EventCallback<GanttDto> OnIndentRight { get; set; }

        private string GetNodeClass() =>
            Task.Children?.Count > 0 ? "wbs-node wbs-parent" : "wbs-node wbs-leaf";

        private void ToggleExpand() => Task.IsExpanded = !Task.IsExpanded;

        private string Truncate(string text, int maxLength) =>
            string.IsNullOrEmpty(text) ? "" :
            text.Length <= maxLength ? text : text[..(maxLength - 3)] + "...";

        private async Task Add(Guid id) =>
            await OnAdd.InvokeAsync(id);

        private async Task Edit(GanttDto task) =>
            await OnEdit.InvokeAsync(task);

        private async Task Delete(GanttDto task) =>
            await OnDelete.InvokeAsync(task);

        private async Task MoveUp(GanttDto task) =>
            await OnMoveUp.InvokeAsync(task);

        private async Task MoveDown(GanttDto task) =>
            await OnMoveDown.InvokeAsync(task);

        private async Task IndentLeft(GanttDto task) =>
            await OnIndentLeft.InvokeAsync(task);

        private async Task IndentRight(GanttDto task) =>
            await OnIndentRight.InvokeAsync(task);

        [Parameter] public List<GanttDto> AllTasks { get; set; } = new();
       

    }
}
