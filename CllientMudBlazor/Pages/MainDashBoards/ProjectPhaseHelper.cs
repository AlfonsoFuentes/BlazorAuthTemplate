using MudBlazor;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages.MainDashBoards
{
    public static class ProjectPhaseHelper
    {
        public static Color GetColor(ProjectStatusEnum status) => status.Id switch
        {
            2 => Color.Success,   // Execution
            1 => Color.Warning,   // Planning
            0 => Color.Primary,   // Created/Start
            _ => Color.Default
        };

        public static string GetIcon(ProjectStatusEnum status) => status.Id switch
        {
            2 => "🟢",
            1 => "✅",
            0 => "🔵",
            3 => "⚫",
            4 => "🗑️",
            _ => "❓"
        };
    }
}
