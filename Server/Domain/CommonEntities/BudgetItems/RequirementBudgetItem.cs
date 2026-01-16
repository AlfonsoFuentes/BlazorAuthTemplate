

namespace Server.Domain.CommonEntities.BudgetItems
{
    public class RequirementBudgetItem : Entity
    {
        public Guid RequirementId { get; set; }
        public Requirement Requirement { get; set; } = null!; // Asumo que esta entidad existe
        public Guid BudgetItemId { get; set; }
        public BudgetItem BudgetItem { get; set; } = null!;
    }
    internal class RequirementBudgetItemConfig : IEntityTypeConfiguration<RequirementBudgetItem>
    {
        public void Configure(EntityTypeBuilder<RequirementBudgetItem> builder)
        {
            builder.HasKey(kb => new { kb.RequirementId, kb.BudgetItemId });

            builder.HasOne(kb => kb.BudgetItem)
                .WithMany(b => b.RequirementBudgetItems)
                .HasForeignKey(kb => kb.BudgetItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ CORRECCIÓN AQUÍ:
            builder.HasOne(kb => kb.Requirement)
                .WithMany(k => k.RequirementBudgetItems) // <--- Debes poner la colección aquí
                .HasForeignKey(kb => kb.RequirementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
