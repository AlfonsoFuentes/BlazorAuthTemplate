// Server/Services/ProjectCacheInvalidator.cs
using Shared.Dtos.BudgetItems;
using Shared.Dtos.Plannings.RiskMatrixs;
using Shared.Dtos.ProjectDefinitions;
using Shared.Dtos.Projects;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.ExpertJudgements;
using Shared.Dtos.Starts.KnownRisks;
using Shared.Dtos.Starts.LearnedLessonsByProjects;
using Shared.Dtos.Starts.Qualitys;
using Shared.Dtos.Starts.Requirements;
using Shared.Dtos.Starts.StakeHolderInsideProjectInsideProjects;
using Shared.Enums.DashBoardTable;
using Shared.Enums.ProjectDefinitionTypes;

// Asegúrate de importar los namespaces de tus Endpoints para usar typeof()
// using Server.Endpoints... 

public static class ProjectCacheBrain
{

    public static string[] GetStartKeyToInvalidate(Guid projectId, Guid Id, DashBoardsStartTable type)
    {
        var keysToKill = new HashSet<string>();
        keysToKill.Add($"{typeof(ExportProjectChartedPDF).Name}-{projectId}");
        keysToKill.Add($"{typeof(GetAllProjectDashBoards).Name}");
        keysToKill.Add($"{typeof(GetProjectDashBoardStartById).Name}-{projectId}");
        keysToKill.Add($"{typeof(GetProjectDashBoardById).Name}-{projectId}");
        var getAll = GetAllStartListKey(type);
        if (!string.IsNullOrEmpty(getAll))
            keysToKill.Add($"{getAll}-{projectId}");
        var getById = GetByIdStartListKey(type);
        if (!string.IsNullOrEmpty(getById))
            keysToKill.Add($"{getById}-{projectId}-{Id}");


        return keysToKill.ToArray();
    }
    public static string[] GetStartKeyToInvalidate(Guid projectId, Guid Id, ProjectDefinitionType definitionType)
    {
        var keysToKill = new HashSet<string>();
        keysToKill.Add($"{typeof(ExportProjectChartedPDF).Name}-{projectId}");
        keysToKill.Add($"{typeof(GetAllProjectDashBoards).Name}");
        keysToKill.Add($"{typeof(GetProjectDashBoardStartById).Name}-{projectId}");
        keysToKill.Add($"{typeof(GetProjectDashBoardById).Name}-{projectId}");
        var getAll = GetAllStartListKey(definitionType);
        if (!string.IsNullOrEmpty(getAll))
            keysToKill.Add($"{getAll}-{projectId}");
        var getById = $"{typeof(GetProjectDefinitionById).Name}";
        if (!string.IsNullOrEmpty(getById))
            keysToKill.Add($"{getById}-{Id}");


        return keysToKill.ToArray();
    }

    // ---------------------------------------------------------
    // HELPERS PRIVADOS PARA MAPEAR ENUM -> KEY ESPECÍFICA
    // ---------------------------------------------------------

    private static string? GetAllStartListKey(DashBoardsStartTable table)
    {
        // Aquí aplicamos tu lógica de "ProjectDefinitions"
        // Mapeamos DashBoardsStartTable -> ProjectDefinitionType string
        return table switch
        {
            DashBoardsStartTable.StakeHolders => $"{typeof(GetAllStakeHolderInsideProjects).Name}",

            DashBoardsStartTable.Requirements => $"{typeof(GetAllRequirements).Name}",

            DashBoardsStartTable.KnownRisks => $"{typeof(GetAllKnownRisks).Name}",
            DashBoardsStartTable.ExpertJudgment => $"{typeof(GetAllExpertJudgements).Name}",

            DashBoardsStartTable.Quality => $"{typeof(GetAllQualitys).Name}",

            DashBoardsStartTable.Investment => $"{typeof(GetAllBudgetItems).Name}",

            DashBoardsStartTable.RiskMatrix => $"{typeof(GetAllRiskMatrixs).Name}",




            _ => null // Si no tiene caché específica de lista
        };
    }
    private static string? GetAllStartListKey(ProjectDefinitionType definitionType)
    {
        // Aquí aplicamos tu lógica de "ProjectDefinitions"
        // Mapeamos DashBoardsStartTable -> ProjectDefinitionType string
        return definitionType switch
        {

            ProjectDefinitionType.Background => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.Background}",
            ProjectDefinitionType.Objective => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.Objective}",

            ProjectDefinitionType.Scope => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.Scope}",

            ProjectDefinitionType.Benefit => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.Benefit}",
            ProjectDefinitionType.AcceptanceCriteria => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.AcceptanceCriteria}",

            ProjectDefinitionType.Constraint => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.Constraint}",
            ProjectDefinitionType.Assumption => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.Assumption}",
            ProjectDefinitionType.Deliverable => $"{typeof(GetAllProjectDefinitions).Name}-{ProjectDefinitionType.Deliverable}",





            _ => null // Si no tiene caché específica de lista
        };
    }
    private static string? GetByIdStartListKey(DashBoardsStartTable table)
    {

        return table switch
        {
            DashBoardsStartTable.StakeHolders => $"{typeof(GetStakeHolderInsideProjectById).Name}",

            DashBoardsStartTable.Requirements => $"{typeof(GetRequirementById).Name}",

            DashBoardsStartTable.KnownRisks => $"{typeof(GetKnownRiskById).Name}",
            DashBoardsStartTable.ExpertJudgment => $"{typeof(GetExpertJudgementById).Name}",


            DashBoardsStartTable.Quality => $"{typeof(GetQualityById).Name}",
            DashBoardsStartTable.LearnedLessons => $"{typeof(GetLearnedLessonsByProjectById).Name}",
            DashBoardsStartTable.Investment => $"{typeof(GetBudgetItemById).Name}",

            DashBoardsStartTable.RiskMatrix => $"{typeof(GetRiskMatrixById).Name}",




            _ => null // Si no tiene caché específica de lista
        };
    }
    private static string? GetByIdStartListKey() => $"{typeof(GetProjectDefinitionById).Name}";

}


