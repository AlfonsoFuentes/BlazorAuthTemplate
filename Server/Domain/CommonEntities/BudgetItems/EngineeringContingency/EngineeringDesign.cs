namespace Server.Domain.CommonEntities.BudgetItems.EngineeringContingency
{
    public class EngineeringDesign : BudgetItem
    {

        public override string Letter { get; set; } = "O";

        [NotMapped]
        public override int OrderList => 14;
        public static EngineeringDesign Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                

            };
        }

    }

}
