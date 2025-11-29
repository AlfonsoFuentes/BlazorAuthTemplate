namespace Server.Domain.CommonEntities.BudgetItems.EngineeringContingency
{
    public interface IEngineeringContingency
    {
        double Percentage { get; set; }
    }
    public class Engineering : BudgetItem, IEngineeringContingency
    {

        public override string Letter { get; set; } = "O";
        public double Percentage { get; set; }
        [NotMapped]
        public override int OrderList => 13;
        public static Engineering Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,

            };
        }

    }

}
