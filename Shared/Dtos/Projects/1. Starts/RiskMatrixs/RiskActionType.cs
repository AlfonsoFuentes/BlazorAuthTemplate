using System.ComponentModel;

namespace Shared.Dtos.Plannings.RiskMatrixs
{
    public enum RiskActionType
    {
        [Description("Mitigation")]
        Mitigation,  // Acciones preventivas (reducir probabilidad/impacto)
        [Description("Contingency")]
        Contingency  // Acciones reactivas (si el riesgo ocurre)
    }
}
