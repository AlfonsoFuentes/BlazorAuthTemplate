namespace Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Valves
{
    public class Valve : BudgetItem
    {
        public override string Letter { get; set; } = "V";

     
        [NotMapped]
        public override int OrderList => 5;
        public static Valve Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,


            };
        }
        //public ICollection<BasicValveItem> ValveItems { get; set; } = new List<BasicValveItem>();
        //protected override double _BudgetUSD => ValveItems.Count == 0 ? _SettedBudgetUSD : ValveItems.Sum(x => x.BudgetUSD);
    }

}
