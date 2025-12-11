using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Alterations
{
    public class AlterationDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

        public double Quantity { get; set; }
        public double UnitaryCost { get; set; }
        public string Nomenclatore => $"A{Order}";
        double _budget => Quantity * UnitaryCost;
       
        public double BudgetUSD
        {
            get { return _budget; }
            set { }
        }

    }
    public class CreateAlteration : AlterationDto
    {

    }
    public class EditAlteration : AlterationDto
    {

    }
    public class GetAllAlterations
    {
        public Guid ProjectId { get; set; }
    }
    public class GetAlterationById
    {
        public Guid Id { set; get; }
    }
    public class ValidateAlterationName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteAlteration
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupAlteration
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderAlteration
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
