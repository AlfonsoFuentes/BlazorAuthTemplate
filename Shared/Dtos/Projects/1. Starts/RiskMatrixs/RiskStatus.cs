using System.ComponentModel;

namespace Shared.Dtos.Plannings.RiskMatrixs
{
    // ✅ ESTADO DEL RIESGO
    public enum RiskStatus
    {
        [Description("Identified (Draft)")]
        Identified = 1,

        [Description("Active (Monitoring & Controlling)")]
        Active = 2,

        [Description("Occurred (Issue / Problem)")]
        Occurred = 3,

        [Description("Closed (No longer a threat)")]
        Closed = 4
    }
}
