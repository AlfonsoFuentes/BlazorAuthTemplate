using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Valves
{
    public class ValveDto : GeneralDto  
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }


        public string Nomenclatore => $"D{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateValve : ValveDto
    {

    }
    public class EditValve : ValveDto
    {

    }
    public class GetAllValves
    {
        public Guid ProjectId { get; set; }
    }
    public class GetValveById
    {
        public Guid Id { set; get; }
    }
    public class ValidateValveName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteValve
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupValve
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderValve
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
