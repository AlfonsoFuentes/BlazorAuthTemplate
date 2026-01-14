using Shared.Attributtes;
using Shared.ExtensionsMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Shared.Enums.BudgetCategorys
{
    public enum BudgetCategory
    {
        // --- EXPENSES ---
        [BudgetMetadata("A", "Alterations")]
        Alteration = 0,

        // --- CAPITAL ---
        [BudgetMetadata("B", "Foundations")]
        Foundation = 1,

        [BudgetMetadata("C", "Structural")]
        Structural = 2,

        [BudgetMetadata("D", "Equipments")]
        Equipment = 3,

        [BudgetMetadata("E", "Electrical")]
        Electrical = 4,

        [BudgetMetadata("F", "Piping")]
        Pipe = 5,

        [BudgetMetadata("G", "Instruments")]
        Instrument = 6,

        [BudgetMetadata("H", "Insulation")]
        Insulation = 7,

        [BudgetMetadata("I", "Painting")]
        Painting = 8,

        [BudgetMetadata("K", "EHS")]
        EHS = 9,

        // --- ESPECIALES ---
        [BudgetMetadata("L", "Taxes")]
        Tax = 10,

        [BudgetMetadata("N", "Testing")]
        Testing = 11,

        [BudgetMetadata("O", "Engineering")]
        Engineering = 12,

        [BudgetMetadata("P", "Contingency")]
        Contingency = 13,

        [BudgetMetadata("V", "Valves")]
        Valve = 14
    }
}
