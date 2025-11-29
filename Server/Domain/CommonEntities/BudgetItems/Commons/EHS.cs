namespace Server.Domain.CommonEntities.BudgetItems.Commons
{
    public class EHS : BudgetItem
    {

        public override string Letter { get; set; } = "K";
        public double UnitaryCost { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public override int OrderList => 10;
        public static EHS Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                

            };
        }
    }

}
