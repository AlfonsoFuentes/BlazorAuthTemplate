using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.WBSs
{
    public partial class WBSCardDisplay
    {
        [Parameter] public GanttDto Task { get; set; } = null!;
        private bool HasConflict => Task.Dependencies?.Any(d => d.IsCircularConflict) ?? false;
    }
}
