using Shared.Dtos.General;

namespace Shared.Dtos.Starts.BackGrounds
{
    public class BackGroundDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateBackGround     : BackGroundDto
    {
        
    }
    public class EditBackGround : BackGroundDto
    {

    }
    public class GetAllBackGrounds
    {
          public Guid ProjectId { get; set; }
    }
    public class GetBackGroundById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateBackGroundName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteBackGround
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupBackGround
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderBackGround
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
