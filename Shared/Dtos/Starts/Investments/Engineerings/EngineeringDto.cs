using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Engineerings
{
    public class EngineeringSalarysDto : GeneralDto
    {
        public Guid Id { get; set; }
        string _Name => $"Engineering {Percentage}%";
        public string Name
        {
            get => _Name;
            set { }
        }
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        double _Percentage;
        public double Percentage
        {
            get { return _Percentage; }
            set
            {
                _Percentage = value;
                if (CapitalBudget > 0 && ContingencyPercentage > 0)
                {
                    BudgetUSD = CapitalBudget * (Percentage / (100 - TotalPercentage));
                }
            }
        }

        public string Nomenclatore => $"O{Order}";
        public double BudgetUSD { get; set; }

        public double CapitalBudget { get; set; }
        public double ContingencyPercentage { get; set; }
        public double TotalPercentage => Percentage + ContingencyPercentage;

    }

    public class EditEngineeringSalarys : EngineeringSalarysDto
    {

    }
    public class GetAllEngineeringSalarys
    {
        public Guid ProjectId { get; set; }
    }
    public class GetEngineeringSalaryById
    {
        public Guid Id { set; get; }
    }
}
