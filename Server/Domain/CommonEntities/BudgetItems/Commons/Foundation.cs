namespace Server.Domain.CommonEntities.BudgetItems.Commons
{
    public class Foundation : BudgetItem
    {

        public override string Letter { get; set; } = "B";
        public double UnitaryCost { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public override int OrderList => 2;
        public static Foundation Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                

            };
        }
    }

}
