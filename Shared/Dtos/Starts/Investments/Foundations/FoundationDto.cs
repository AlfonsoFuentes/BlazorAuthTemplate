using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Foundations
{
    public class FoundationDto : GeneralDto  
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

        

        public string Nomenclatore => $"D{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateFoundation : FoundationDto
    {

    }
    public class EditFoundation : FoundationDto
    {

    }
    public class GetAllFoundations
    {
        public Guid ProjectId { get; set; }
    }
    public class GetFoundationById
    {
        public Guid Id { set; get; }
    }
    public class ValidateFoundationName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteFoundation
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupFoundation
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderFoundation
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
