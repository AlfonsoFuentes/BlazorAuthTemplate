using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Electricals
{
    public class ElectricalDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

  

        public string Nomenclatore => $"E{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateElectrical : ElectricalDto
    {

    }
    public class EditElectrical : ElectricalDto
    {

    }
    public class GetAllElectricals
    {
        public Guid ProjectId { get; set; }
    }
    public class GetElectricalById
    {
        public Guid Id { set; get; }
    }
    public class ValidateElectricalName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteElectrical
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupElectrical
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderElectrical
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
