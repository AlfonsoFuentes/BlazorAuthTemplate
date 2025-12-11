using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;

namespace Shared.Dtos.Starts.EHSs
{
    public class EHSDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

    

        public string Nomenclatore => $"K{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateEHS : EHSDto
    {

    }
    public class EditEHS : EHSDto
    {

    }
    public class GetAllEHSs
    {
        public Guid ProjectId { get; set; }
    }
    public class GetEHSById
    {
        public Guid Id { set; get; }
    }
    public class ValidateEHSName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteEHS
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupEHS
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderEHS
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
