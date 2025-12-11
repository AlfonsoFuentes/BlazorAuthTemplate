using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Instruments
{
    public class InstrumentDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }



        public string Nomenclatore => $"G{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateInstrument : InstrumentDto
    {

    }
    public class EditInstrument : InstrumentDto
    {

    }
    public class GetAllInstruments
    {
        public Guid ProjectId { get; set; }
    }
    public class GetInstrumentById
    {
        public Guid Id { set; get; }
    }
    public class ValidateInstrumentName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteInstrument
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupInstrument
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderInstrument
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
