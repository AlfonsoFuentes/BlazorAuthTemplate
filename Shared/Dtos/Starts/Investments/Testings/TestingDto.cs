using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Testings
{
    public class TestingDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

      

        public string Nomenclatore => $"N{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreateTesting : TestingDto
    {

    }
    public class EditTesting : TestingDto
    {

    }
    public class GetAllTestings
    {
        public Guid ProjectId { get; set; }
    }
    public class GetTestingById
    {
        public Guid Id { set; get; }
    }
    public class ValidateTestingName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteTesting
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupTesting
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderTesting
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
