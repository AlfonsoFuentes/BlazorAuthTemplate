using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;

namespace Shared.Dtos.Starts.Contingencys
{
    public class ContingencyDto : GeneralDto
    {
        public Guid Id { get; set; }
        string _Name => $"Contingency {Percentage}%";
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
                if (CapitalBudget > 0 && EngineeringPercentage > 0)
                {
                    BudgetUSD = CapitalBudget * (Percentage / (100 - TotalPercentage));
                }
            }
        }

        public string Nomenclatore => $"O{Order}";
        public double BudgetUSD { get; set; }

        public double CapitalBudget { get; set; }
        public double EngineeringPercentage { get; set; }
        public double TotalPercentage => Percentage + EngineeringPercentage;

    }

    public class EditContingency : ContingencyDto
    {

    }
    public class GetAllContingencys
    {
        public Guid ProjectId { get; set; }
    }
    public class GetContingencyById
    {
        public Guid Id { set; get; }
    }

}
