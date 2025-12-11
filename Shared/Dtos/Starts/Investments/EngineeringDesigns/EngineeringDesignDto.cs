using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.EngineeringDesigns
{
    public class EngineeringDesignDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }


        public string Nomenclatore => $"O{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateEngineeringDesign : EngineeringDesignDto
    {

    }
    public class EditEngineeringDesign : EngineeringDesignDto
    {

    }
    public class GetAllEngineeringDesigns
    {
        public Guid ProjectId { get; set; }
    }
    public class GetEngineeringDesignById
    {
        public Guid Id { set; get; }
    }
    public class ValidateEngineeringDesignName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteEngineeringDesign
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupEngineeringDesign
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderEngineeringDesign
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
