using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Equipments
{
    public class EquipmentDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }


        public string Nomenclatore => $"D{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateEquipment : EquipmentDto
    {

    }
    public class EditEquipment : EquipmentDto
    {

    }
    public class GetAllEquipments
    {
        public Guid ProjectId { get; set; }
    }
    public class GetEquipmentById
    {
        public Guid Id { set; get; }
    }
    public class ValidateEquipmentName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteEquipment
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupEquipment
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderEquipment
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
