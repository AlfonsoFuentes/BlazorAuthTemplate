using Shared.Enums.ProjectNeedTypes;

namespace Shared.Dtos.Projects
{
    public class ProjectDashboardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ProjectStatusEnum Status { get; set; }=ProjectStatusEnum.None;
        public DateTime? LastModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public double CompletionPercentage { get; set; } = 0;
        public double SpentUSD { get; set; } = 0;
        public double TotalBudgetUSD { get; set; } = 0;
        public DateTime? NextMilestoneDate { get; set; }

        public string Phase => Status.Id switch
        {
            2 => "EXECUTION",
            1 => "PLANNING",
            0 => "START",
            _ => "UNKNOWN"
        };
    }
    public class GetAllProjectDashBoards
    {

    }
    public class GetProjectDashBoardById
    {
        public Guid Id { set; get; }
    }
}
