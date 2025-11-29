namespace Server.Domain.CommonEntities.BudgetItems.Commons
{
    public class Electrical : BudgetItem
    {

        public override string Letter { get; set; } = "E";
        public double UnitaryCost { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public override int OrderList => 6;
        public static Electrical Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                

            };
        }
    }

}
