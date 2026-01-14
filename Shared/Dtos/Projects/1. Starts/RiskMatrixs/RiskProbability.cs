using System.ComponentModel;

namespace Shared.Dtos.Plannings.RiskMatrixs
{
    // ✅ ESCALA PMP DE PROBABILIDAD (1-5)
    public enum RiskProbability
    {
        [Description("Select Probability...")]
        None = 0,

        [Description("1. Very Unlikely (Once in 10 years / < 5%)")]
        VeryUnlikely = 1,

        [Description("2. Unlikely (Once in 5 years / 5-20%)")]
        Unlikely = 2,

        [Description("3. Possible (Once in 2 years / 21-50%)")]
        Possible = 3,

        [Description("4. Likely (Once a year / 51-80%)")]
        Likely = 4,

        [Description("5. Very Likely (Multiple times a year / > 80%)")]
        VeryLikely = 5
    }
}
