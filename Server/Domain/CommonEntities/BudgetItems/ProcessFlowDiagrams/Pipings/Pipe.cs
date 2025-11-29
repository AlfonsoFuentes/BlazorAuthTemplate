namespace Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Pipings
{
    public class Pipe : BudgetItem
    {
        
        public override string Letter { get; set; } = "F";

  

        [NotMapped]
        public override int OrderList => 7;
        public static Pipe Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
             

            };
        }
        //protected override double _BudgetUSD  => PipeItems.Count == 0 ? _SettedBudgetUSD : PipeItems.Sum(x => x.BudgetUSD);
        //public ICollection<BasicPipeItem> PipeItems { get; set; } = new List<BasicPipeItem>();
    }


}
