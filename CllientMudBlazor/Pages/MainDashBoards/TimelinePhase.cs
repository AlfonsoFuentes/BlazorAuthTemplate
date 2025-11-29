using MudBlazor;
using MudBlazor.Utilities;

namespace CllientMudBlazor.Pages.MainDashBoards
{
    public class TimelinePhase
    {
        public string Name { get; init; } = string.Empty;
        public Color Color { get; init; } 
        public Variant Variant { get; init; }
        public string Status { get; init; } = string.Empty;
        public string Icon { get; init; } = "•";
    }
    public static class PhaseFactory
    {
        // ✅ Colores de tu paleta #6 (Venngage)
        

        // ✅ START — Lavanda suave (base de la paleta)
        public static TimelinePhase Start(string status = "Draft") =>
            new()
            {
                Name = "START",
                Color = Color.Primary,
                Variant = Variant.Filled,
                Status = status,
                Icon = "🔵"
            };

        // ✅ PLANNING — Melocotón cálido (advertencia suave)
        public static TimelinePhase Planning(string status = "In Review") =>
            new()
            {
                Name = "PLANNING",
                Color = Color.Secondary,
                Variant = Variant.Filled,
                Status = status,
                Icon = "🟠"
            };

        // ✅ EXECUTION — Menta fresca (éxito en progreso)
        public static TimelinePhase Execution(string status = "In Progress") =>
            new()
            {
                Name = "EXECUTION",
                Color = Color.Success,
                Variant = Variant.Filled,
                Status = status,
                Icon = "🟢"
            };

        // ✅ MONITORING — Lavanda claro (neutral, en observación)
        public static TimelinePhase Monitoring(string status = "Pending") =>
            new()
            {
                Name = "MONITORING",
                Color = Color.Success,
                Variant = Variant.Filled,
                Status = status,
                Icon = "⚪"
            };

        // ✅ CLOSING — Rosa terracota (cierre, suave y positivo)
        public static TimelinePhase Closing(string status = "Closed") =>
            new()
            {
                Name = "CLOSING",
                Color = Color.Info,
                Variant = Variant.Filled,
                Status = status,
                Icon = "✅"
            };
    }
}
