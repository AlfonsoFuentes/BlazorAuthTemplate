using Shared.Enums.CostCenter;
using Shared.Enums.Focuses;
using Shared.Enums.ProjectNeedTypes;

namespace Shared.Dtos.Projects
{
    public class ProjectDashboardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ProjectStatusEnum Status { get; set; }=ProjectStatusEnum.None;
        public DateTime? LastModifiedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public int LastVisitedPhase { get; set; } = 1;
        public string ProjectCode { get; set; } = string.Empty;
       
    }
    public class ProjectDashboardStartDto: ProjectDashboardDto
    {
        public decimal CapitalUSD { get; set; }       // CAPEX
        public decimal ExpensesUSD { get; set; }      // OPEX
        public decimal AppropriationUSD => CapitalUSD + ExpensesUSD; // Apropiación total
        public bool HasBusinessCase { get; set; }

        // ¿Están claros los Objetivos?
        public bool HasObjectives { get; set; }

        // ¿Se definió el Alcance/Entregables?
        public bool HasScope { get; set; }
        // ¿Se definió el Alcance/Entregables?
        public bool HasRequirements { get; set; }

        // ¿Hay Interesados identificados?
        public bool HasStakeholders { get; set; }

        // ¿Se evaluaron Riesgos iniciales?
        public bool HasRisks { get; set; }

        // --- 3. METRICS (Para mostrar números en las tarjetas) ---
        public int StakeholderCount { get; set; }
        public int HighRiskCount { get; set; }
        public int ObjectivesCount { get; set; }

        // Propiedad calculada: ¿Está listo para aprobar?
        public bool IsReadyToPlan =>
            HasBusinessCase && HasObjectives && HasScope &&
            HasStakeholders && HasRisks && AppropriationUSD > 0;
    }
    public class ProjectDashboardPlanningDto : ProjectDashboardDto
    {

    }
    public class ProjectDashboardExecutingDto : ProjectDashboardDto
    {

    }
    public class ProjectDashboardMonitoringDto : ProjectDashboardDto
    {

    }
    public class ProjectDashboardClosingDto : ProjectDashboardDto
    {

    }
    public class GetAllProjectDashBoards
    {

    }
    public class GetProjectDashBoardById
    {
        public Guid Id { set; get; }
    }
    public class GetProjectDashBoardStartById
    {
        public Guid Id { set; get; }
    }
    public class GetProjectDashBoardPlanningById
    {
        public Guid Id { set; get; }
    }
    public class GetProjectDashBoardExecutingById
    {
        public Guid Id { set; get; }
    }
    public class GetProjectDashBoardMonitoringById
    {
        public Guid Id { set; get; }
    }
    public class GetProjectDashBoardClosingById
    {
        public Guid Id { set; get; }
    }
    public class UpdateProjectLastPhaseDto
    {
        public Guid ProjectId { get; set; }
        public int PhaseId { get; set; } // 1=Start, 2=Planning, etc.
    }
}
