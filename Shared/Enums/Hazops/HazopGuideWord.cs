using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Enums.Hazops
{
    public enum HazopGuideWord
    {
        None = 0,
        No,
        Less,
        More,
        AsWellAs,
        PartOf,
        Reverse,
        OtherThan
    }

    public enum HazopParameter
    {
        None,
        Flow,
        Pressure,
        Temperature,
        Level,
        Viscosity,
        Composition,
        Addition,
        Reaction
    }
}
