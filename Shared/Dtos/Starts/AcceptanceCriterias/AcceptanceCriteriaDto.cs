using Shared.Dtos.General;
using Shared.Interfaces;

namespace Shared.Dtos.Starts.AcceptanceCriterias
{
    public class AcceptanceCriteriaDto : GeneralDto, IHasId
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

    }
    public class CreateAcceptanceCriteria : AcceptanceCriteriaDto
    {

    }
    public class EditAcceptanceCriteria : AcceptanceCriteriaDto
    {

    }
    public class GetAllAcceptanceCriterias
    {
        public Guid ProjectId { get; set; }
    }
    public class GetAcceptanceCriteriaById
    {
        public Guid Id { set; get; }
    }
    public class ValidateAcceptanceCriteriaName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteAcceptanceCriteria
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupAcceptanceCriteria
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderAcceptanceCriteria
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
