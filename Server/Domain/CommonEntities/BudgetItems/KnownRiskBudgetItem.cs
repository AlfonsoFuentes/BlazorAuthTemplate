

namespace Server.Domain.CommonEntities.BudgetItems
{
    // 3. Relación con KNOWN RISK (Riesgos Conocidos/Lecciones Aprendidas)
    public class KnownRiskBudgetItem : Entity
    {
        public Guid KnownRiskId { get; set; }
        public KnownRisk KnownRisk { get; set; } = null!; // Asumo que esta entidad existe
        public Guid BudgetItemId { get; set; }
        public BudgetItem BudgetItem { get; set; } = null!;
    }
    internal class KnownRiskBudgetItemConfig : IEntityTypeConfiguration<KnownRiskBudgetItem>
    {
        public void Configure(EntityTypeBuilder<KnownRiskBudgetItem> builder)
        {
            builder.HasKey(kb => new { kb.KnownRiskId, kb.BudgetItemId });

            builder.HasOne(kb => kb.BudgetItem)
                .WithMany(b => b.KnownRiskBudgetItems)
                .HasForeignKey(kb => kb.BudgetItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ CORRECCIÓN AQUÍ:
            builder.HasOne(kb => kb.KnownRisk)
                .WithMany(k => k.KnownRiskBudgetItems) // <--- Debes poner la colección aquí
                .HasForeignKey(kb => kb.KnownRiskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
