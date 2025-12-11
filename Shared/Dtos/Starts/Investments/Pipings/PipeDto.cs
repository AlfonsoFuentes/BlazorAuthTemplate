using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Pipes
{
    public class PipeDto : GeneralDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }



        public string Nomenclatore => $"F{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreatePipe : PipeDto
    {

    }
    public class EditPipe : PipeDto
    {

    }
    public class GetAllPipes
    {
        public Guid ProjectId { get; set; }
    }
    public class GetPipeById
    {
        public Guid Id { set; get; }
    }
    public class ValidatePipeName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeletePipe
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupPipe
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderPipe
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
