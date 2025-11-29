using Shared.Dtos.General;

namespace Shared.Dtos.Starts.Constrainsts
{
    public class ConstrainstDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateConstrainst     : ConstrainstDto
    {
        
    }
    public class EditConstrainst : ConstrainstDto
    {

    }
    public class GetAllConstrainsts
    {
          public Guid ProjectId { get; set; }
    }
    public class GetConstrainstById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateConstrainstName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteConstrainst
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupConstrainst
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderConstrainst
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
