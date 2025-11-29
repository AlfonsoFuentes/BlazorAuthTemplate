namespace Server.Domain.CommonEntities.BudgetItems.Commons
{
    public class Testing : BudgetItem
    {

        public override string Letter { get; set; } = "N";
        public double UnitaryCost { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public override int OrderList => 12;
        public static Testing Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
            

            };
        }
    }

}
