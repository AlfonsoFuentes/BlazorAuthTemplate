namespace Server.Domain.CommonEntities.BudgetItems.Taxes
{
    public class TaxesItem : Entity
    {

        public Guid TaxItemId { get; set; }
        public Tax TaxItem { get; set; } = null!;

        public Guid? SelectedId { get; set; }
        public BudgetItem? Selected { get; set; } = null!;
        

        public static TaxesItem Create(Guid TaxId, Guid BudgetItemId)
        {
            return new()
            {
                TaxItemId = TaxId,
                SelectedId = BudgetItemId,
            };
        }
    }

}
