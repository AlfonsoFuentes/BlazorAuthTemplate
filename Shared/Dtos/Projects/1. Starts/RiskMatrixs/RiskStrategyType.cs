using System.ComponentModel;

namespace Shared.Dtos.Plannings.RiskMatrixs
{
    // ✅ ESTRATEGIAS DE RESPUESTA
    public enum RiskStrategyType
    {
        [Description("Select Strategy...")]
        None = 0,

        [Description("Avoid (Eliminate the threat or cause)")]
        Avoid = 1,

        [Description("Mitigate (Reduce probability or impact)")]
        Mitigate = 2,

        [Description("Transfer (Shift responsibility e.g. Insurance)")]
        Transfer = 3,

        [Description("Accept (Acknowledge and monitor)")]
        Accept = 4,

        [Description("Exploit (Ensure opportunity happens)")]
        Exploit = 5
    }
}
