namespace Server.Domain.CommonEntities.BudgetItems.Commons
{
    public class Structural : BudgetItem
    {

        public override string Letter { get; set; } = "C";
        public double UnitaryCost { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public override int OrderList => 3;
        public static Structural Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                
            };
        }
    }

}
