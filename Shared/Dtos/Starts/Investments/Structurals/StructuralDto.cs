using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Structurals
{
    public class StructuralDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

        public double Quantity { get; set; }
        public double UnitaryCost { get; set; }

        public string Nomenclatore => $"C{Order}";
        double _budget => Quantity * UnitaryCost;

        public double BudgetUSD
        {
            get { return _budget; }
            set { }
        }

    }
    public class CreateStructural : StructuralDto
    {

    }
    public class EditStructural : StructuralDto
    {

    }
    public class GetAllStructurals
    {
        public Guid ProjectId { get; set; }
    }
    public class GetStructuralById
    {
        public Guid Id { set; get; }
    }
    public class ValidateStructuralName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteStructural
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupStructural
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderStructural
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
