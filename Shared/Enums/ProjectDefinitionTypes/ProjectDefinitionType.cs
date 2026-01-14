using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Shared.Enums.ProjectDefinitionTypes
{
    public enum ProjectDefinitionType
    {
        [Description("Background")]
        Background = 1,
        [Description("Objective")]
        Objective = 2,
        [Description("Scope")]
        Scope = 3,
        [Description("Benefit")]
        Benefit = 4,
        [Description("Constraint")]
        Constraint = 5,
        [Description("Assumption")]
        Assumption = 6,
        [Description("Acceptance Criteria")]
        AcceptanceCriteria = 7,
        [Description("Deliverable")]
        Deliverable = 8,
       
      
    }
}
