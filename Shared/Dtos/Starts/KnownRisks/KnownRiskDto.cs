using Shared.Dtos.General;

namespace Shared.Dtos.Starts.KnownRisks
{
    public class KnownRiskDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateKnownRisk     : KnownRiskDto
    {
        
    }
    public class EditKnownRisk : KnownRiskDto
    {

    }
    public class GetAllKnownRisks
    {
          public Guid ProjectId { get; set; }
    }
    public class GetKnownRiskById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateKnownRiskName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteKnownRisk
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupKnownRisk
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderKnownRisk
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
