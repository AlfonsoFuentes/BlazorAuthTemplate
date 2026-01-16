using Shared.Dtos.BudgetItems;
using Shared.Dtos.General;
using Shared.Dtos.StakeHolders;
using Shared.Enums.RequirementPrioritys;
using Shared.Interfaces;

namespace Shared.Dtos.Starts.Requirements
{
    public class RequirementDto : IModelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        public RequirementTypeEnum Type { set; get; } = RequirementTypeEnum.None;
        public StakeHolderDto? RequestedBy { get; set; } = null!;
        public string RequestedByName => RequestedBy != null ? RequestedBy.Name : string.Empty;
        public RequirementPriorityEnum Priority { set; get; } = RequirementPriorityEnum.None;
        public StakeHolderDto? Responsible { get; set; } = null!;
        public string ResponsibleName => Responsible != null ? Responsible.Name : string.Empty;
        public List<BudgetItemDto> LinkedInvestments { get; set; } = new();
    }
    public class CreateRequirement : RequirementDto
    {

    }
    public class EditRequirement : RequirementDto
    {

    }
    public class GetAllRequirements
    {
        public Guid ProjectId { get; set; }
    }
    public class GetRequirementById
    {
        public Guid Id { set; get; }
    }
    public class ValidateRequirementName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteRequirement
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupRequirement
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderRequirement
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
