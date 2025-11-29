using Shared.Dtos.General;

namespace Shared.Dtos.Starts.Objectives
{
    public class ObjectiveDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateObjective     : ObjectiveDto
    {
        
    }
    public class EditObjective : ObjectiveDto
    {

    }
    public class GetAllObjectives
    {
          public Guid ProjectId { get; set; }
    }
    public class GetObjectiveById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateObjectiveName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteObjective
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupObjective
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderObjective
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
