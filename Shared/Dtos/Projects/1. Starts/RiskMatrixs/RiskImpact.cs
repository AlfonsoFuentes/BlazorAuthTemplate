using System.ComponentModel;

namespace Shared.Dtos.Plannings.RiskMatrixs
{
    // ✅ ESCALA PMP DE IMPACTO (1-5)
    public enum RiskImpact
    {
        [Description("Select Impact...")]
        None = 0,

        [Description("1. Negligible (Insignificant cost/schedule impact)")]
        Negligible = 1,

        [Description("2. Minor (< 5% Cost increase / Minor delay)")]
        Minor = 2,

        [Description("3. Moderate (5-10% Cost increase / Milestone delay)")]
        Moderate = 3,

        [Description("4. Major (10-20% Cost increase / Critical path impacted)")]
        Major = 4,

        [Description("5. Critical (> 20% Cost increase / Project failure)")]
        Critical = 5
    }
}
