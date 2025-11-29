using Shared.Dtos.General;

namespace Shared.Dtos.Starts.Assumptions
{
    public class AssumptionDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateAssumption     : AssumptionDto
    {
        
    }
    public class EditAssumption : AssumptionDto
    {

    }
    public class GetAllAssumptions
    {
          public Guid ProjectId { get; set; }
    }
    public class GetAssumptionById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateAssumptionName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteAssumption
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupAssumption
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderAssumption
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
