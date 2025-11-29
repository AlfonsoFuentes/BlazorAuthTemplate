namespace Server.Domain.CommonEntities.BudgetItems.EngineeringContingency
{
    public class Contingency : BudgetItem, IEngineeringContingency
    {

        public override string Letter { get; set; } = "P";
        public double Percentage { get; set; }
        [NotMapped]
        public override int OrderList => 15;
        public static Contingency Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,

            };
        }

    }

}
