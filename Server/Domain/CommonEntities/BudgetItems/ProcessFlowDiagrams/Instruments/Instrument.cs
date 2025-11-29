namespace Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Instruments
{
    public class Instrument : BudgetItem
    {


        public override string Letter { get; set; } = "G";
        
        //protected override double _BudgetUSD => InstrumentItems.Count == 0 ? _SettedBudgetUSD : InstrumentItems.Sum(x => x.BudgetUSD);
        [NotMapped]
        public override int OrderList => 8;
        public static Instrument Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,


            };
        }
        //public ICollection<BasicInstrumentItem> InstrumentItems { get; set; } = new List<BasicInstrumentItem>();
    }

}
