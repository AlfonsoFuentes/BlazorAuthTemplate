namespace Server.Domain.CommonEntities.BudgetItems.Taxes
{
    public class Tax : BudgetItem
    {

        public override string Letter { get; set; } = "L";
        public double Percentage { get; set; }
        public ICollection<TaxesItem> TaxesItems { get; set; } = new List<TaxesItem>();
        [NotMapped]
        public override int OrderList => 11;
        public static Tax Create(Guid ProjectId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
               

            };
        }
    }

}
