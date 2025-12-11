using Shared.Dtos.General;
using Shared.Enums.CostCenter;
using Shared.Enums.Focuses;
using Shared.Enums.ProjectNeedTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Projects
{
    public class ProjectDto
    {

    }
    public class GetProjectToApproveById
    {
        public Guid Id { set; get; }
    }
    public class CreateProject
    {
        public double PercentageEngineering { get; set; }
        public double PercentageContingency { get; set; }

        public double PercentageTaxProductive { get; set; }
        public bool IsProductiveAsset { get; set; } = true;
        public CostCenterEnum CostCenter { get; set; } = CostCenterEnum.None;
        public FocusEnum Focus { get; set; } = FocusEnum.None;
        public ProjectNeedTypeEnum ProjectNeedType { get; set; } = ProjectNeedTypeEnum.None;
        public string ProjectName { get; set; } = string.Empty;
    }
    public class ApproveProject
    {
        public Guid Id { get; set; }
        public double PercentageEngineering { get; set; }
        public double PercentageContingency { get; set; }

        public double PercentageTaxProductive { get; set; }
        public bool IsProductiveAsset { get; set; } = true;
        public CostCenterEnum CostCenter { get; set; } = CostCenterEnum.None;
        public FocusEnum Focus { get; set; } = FocusEnum.None;
        public ProjectNeedTypeEnum ProjectNeedType { get; set; } = ProjectNeedTypeEnum.None;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectNumber { get; set; } = string.Empty;
        public DateTime? InitialProjectDate { get; set; } = DateTime.Today;
        public int BudgetItems { get; set; }
        public int Stakeholders { get; set; }
        public int Requirements { get; set; }
        public int Objectives { get; set; }
        public int Scopes { get; set; }
        public int AcceptenceCriterias { get; set; }
        public int Backgrounds { get; set; }


    }
    public class ExportProjectChartedPDF
    {
        public Guid ProjectId { get; set; }

    }


    public class ValidateProjectName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
    }
    public class ValidateProjectNumber
    {
        public Guid Id { set; get; }
        public string ProjectNumber { set; get; } = string.Empty;
    }

    public class DeleteProject
    {
        public Guid Id { set; get; }
    }

}
