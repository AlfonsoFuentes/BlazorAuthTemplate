using Shared.Dtos.General;

namespace Shared.Dtos.Starts.Scopes
{
    public class ScopeDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateScope     : ScopeDto
    {
        
    }
    public class EditScope : ScopeDto
    {

    }
    public class GetAllScopes
    {
          public Guid ProjectId { get; set; }
    }
    public class GetScopeById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateScopeName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteScope
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupScope
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderScope
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
