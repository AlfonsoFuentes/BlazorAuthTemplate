using Shared.Dtos.General;
using Shared.Dtos.StakeHolders;

namespace Shared.Dtos.Starts.ExpertJudgements
{
    public class ExpertJudgementDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        public StakeHolderDto? Expert { get; set; } = null!;
        public string ExpertName => Expert != null ? Expert.Name : string.Empty;

    }
    public class CreateExpertJudgement : ExpertJudgementDto
    {

    }
    public class EditExpertJudgement : ExpertJudgementDto
    {

    }
    public class GetAllExpertJudgements
    {
        public Guid ProjectId { get; set; }
    }
    public class GetExpertJudgementById
    {
        public Guid Id { set; get; }
    }
    public class ValidateExpertJudgementName
    {
        public Guid Id { set; get; }
        public Guid ExpertId { set; get; }
        public Guid ProjectId { set; get; }
    }

    public class DeleteExpertJudgement
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupExpertJudgement
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderExpertJudgement
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
