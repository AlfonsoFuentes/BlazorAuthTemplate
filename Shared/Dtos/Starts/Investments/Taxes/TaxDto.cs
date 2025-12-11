using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;

namespace Shared.Dtos.Starts.Taxs
{
    public class TaxDto : GeneralDto
    {
        public Guid Id { get; set; }
        string _name=>$"Tax {Percentage}%";
        public string Name
        {
            get
            {
                return _name;
            }
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
                if (CapitalBudget > 0)
                {
                    BudgetUSD = CapitalBudget * (Percentage / 100);
                }
            }
        }

        public string Nomenclatore => $"O{Order}";
        public double BudgetUSD { get; set; }

        public double CapitalBudget { get; set; }
    }

    public class EditTax : TaxDto
    {

    }
    public class GetAllTaxs
    {
        public Guid ProjectId { get; set; }
    }
    public class GetTaxById
    {
        public Guid Id { set; get; }
    }



}
