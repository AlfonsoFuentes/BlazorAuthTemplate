using Shared.Dtos.General;

namespace Shared.Dtos.Starts.Requirements
{
    public class RequirementDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateRequirement     : RequirementDto
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
        public Guid Id {  set; get; }
    }
    public class ValidateRequirementName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
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
