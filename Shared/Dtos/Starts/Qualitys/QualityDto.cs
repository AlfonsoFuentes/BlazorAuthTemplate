using Shared.Dtos.General;

namespace Shared.Dtos.Starts.Qualitys
{
    public class QualityDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateQuality     : QualityDto
    {
        
    }
    public class EditQuality : QualityDto
    {

    }
    public class GetAllQualitys
    {
          public Guid ProjectId { get; set; }
    }
    public class GetQualityById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateQualityName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteQuality
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupQuality
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderQuality
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
