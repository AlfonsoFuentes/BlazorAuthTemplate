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
            ProjectStatusEnum.CREATED_ID => Icons.Material.Filled.Lightbulb,       // Idea / Inicio
            ProjectStatusEnum.PLANNING_ID => Icons.Material.Filled.Architecture,   // Planificación / Diseño
            ProjectStatusEnum.EXECUTION_ID => Icons.Material.Filled.RocketLaunch,  // Ejecución / Despegue
            ProjectStatusEnum.CLOSED_ID => Icons.Material.Filled.TaskAlt,          // Completado / Check
            ProjectStatusEnum.DISCARTED_ID => Icons.Material.Filled.DeleteSweep,   // Descartado
            _ => Icons.Material.Filled.HelpOutline
        };
        public static string GetBackgroundColorStyle(ProjectStatusEnum status) => status.Id switch
        {
            // Azul Corporativo (Suave)
            ProjectStatusEnum.CREATED_ID => "background-color: #1E88E5; color: white;",

            // Púrpura/Indigo (Sofisticado para planeación)
            ProjectStatusEnum.PLANNING_ID => "background-color: #5E35B1; color: white;",

            // Verde Esmeralda (Moderno para ejecución, no el verde "Success" chillón)
            ProjectStatusEnum.EXECUTION_ID => "background-color: #00897B; color: white;",

            // Gris Azulado (Para cerrado, denota archivo/finalizado)
            ProjectStatusEnum.CLOSED_ID => "background-color: #546E7A; color: white;",

            // Rojo suave/Terracota (Para descartado)
            ProjectStatusEnum.DISCARTED_ID => "background-color: #D84315; color: white;",

            _ => "background-color: #9E9E9E; color: white;"
        };

        // 3. COLOR (Enum MudBlazor): Por si necesitas usarlo en componentes simples como Chips
        public static Color GetMudColor(ProjectStatusEnum status) => status.Id switch
        {
            ProjectStatusEnum.EXECUTION_ID => Color.Success,
            ProjectStatusEnum.PLANNING_ID => Color.Info, // Warning suele ser amarillo feo, Info es mejor
            ProjectStatusEnum.CREATED_ID => Color.Primary,
            ProjectStatusEnum.DISCARTED_ID => Color.Error,
            ProjectStatusEnum.CLOSED_ID => Color.Dark,
            _ => Color.Default
        };
    }
}
