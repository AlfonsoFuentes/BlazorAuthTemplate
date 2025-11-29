namespace Server.Domain.CommonEntities.BudgetItems.Commons
{
    public class Painting : BudgetItem
    {

        public override string Letter { get; set; } = "I";
        public double UnitaryCost { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public override int OrderList => 9;
        public static Painting Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,


            };
        }
    }

}
