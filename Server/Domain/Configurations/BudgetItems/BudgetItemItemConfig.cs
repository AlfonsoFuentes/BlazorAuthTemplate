using Server.Domain.CommonEntities.BudgetItems;

namespace Server.Domain.Configurations.BudgetItems
{
    internal class BudgetItemItemConfig : IEntityTypeConfiguration<BudgetItem>
    {
        public void Configure(EntityTypeBuilder<BudgetItem> builder)
        {

            builder.HasMany(x => x.PurchaseOrderItems)
                .WithOne(x => x.BudgetItem)
                .HasForeignKey(x => x.BudgetItemId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
