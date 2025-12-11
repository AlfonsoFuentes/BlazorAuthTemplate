namespace CllientMudBlazor.Services.Enums
{

    using System.ComponentModel;
    using System.Reflection;

    public enum DashBoardsStartTable
    {
        [Description("Stakeholders")]
        StakeHolders,

        [Description("Background")]
        BackGrounds,

        [Description("Objectives")]
        Objectives,

        [Description("Requirements")]
        Requirements,

        [Description("Scope")]
        Scopes,

        [Description("Investment")]
        Investment,

        [Description("Benefits")]
        Benefits,  

        [Description("Acceptance Criteria")]
        AcceptanceCriterias,

        [Description("Constraints")]
        Constrainsts,  

        [Description("Assumptions")]
        Assumptions,

        [Description("Expert Judgment")]
        ExpertJudgment,

        [Description("Quality")]
        Quality,

        [Description("Known Risks")]
        KnownRisks
    }



}
