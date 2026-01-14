using Shared.Dtos.Projects;

namespace Server.Cachekeys
{
    // Shared/Constants/CacheKeys.cs
    public static class ProjectCacheKeys
    {
        // ✅ USAMOS 'nameof': Si renombras la clase del endpoint, esto se actualiza solo.
        public const string DashboardStart = nameof(GetProjectDashBoardStartById);
        public const string DashboardPlanning = nameof(GetProjectDashBoardPlanningById);
        public const string ProjectHeader = nameof(GetProjectDashBoardById);
        public const string ProjectList = nameof(GetAllProjectDashBoards);

        // 🧠 HELPER: Para construir la llave completa con el ID
        // Uso: ProjectCacheKeys.GetKey(ProjectCacheKeys.DashboardStart, projectId)
        public static string GetKey(string baseKey, Guid id) => $"{baseKey}-{id}";
    }
}
