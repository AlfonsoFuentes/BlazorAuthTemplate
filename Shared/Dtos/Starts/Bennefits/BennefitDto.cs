using Shared.Dtos.General;

namespace Shared.Dtos.Starts.Bennefits
{
    public class BennefitDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateBennefit     : BennefitDto
    {
        
    }
    public class EditBennefit : BennefitDto
    {

    }
    public class GetAllBennefits
    {
          public Guid ProjectId { get; set; }
    }
    public class GetBennefitById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateBennefitName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteBennefit
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupBennefit
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderBennefit
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
