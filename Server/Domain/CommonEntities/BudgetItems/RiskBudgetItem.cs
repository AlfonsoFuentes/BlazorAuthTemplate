

namespace Server.Domain.CommonEntities.BudgetItems
{
    public class RiskBudgetItem : Entity
    {
        public Guid RiskMatrixId { get; set; }
        public RiskMatrix RiskMatrix { get; set; } = null!;
        public Guid BudgetItemId { get; set; }
        public BudgetItem BudgetItem { get; set; } = null!;
    }
    internal class RiskBudgetItemConfig : IEntityTypeConfiguration<RiskBudgetItem>
    {
        public void Configure(EntityTypeBuilder<RiskBudgetItem> builder)
        {
            builder.HasKey(rb => new { rb.RiskMatrixId, rb.BudgetItemId });
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder.HasOne(rb => rb.BudgetItem)
              .WithMany(b => b.RiskBudgetItems)
              .HasForeignKey(rb => rb.BudgetItemId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rb => rb.RiskMatrix)
                .WithMany(r => r.RiskBudgetItems)
                .HasForeignKey(rb => rb.RiskMatrixId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict es más seguro aquí para no borrar Riesgos por accidente
        }

    }
}
