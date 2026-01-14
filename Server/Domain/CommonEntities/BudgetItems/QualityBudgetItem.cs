

namespace Server.Domain.CommonEntities.BudgetItems
{
    // 2. Relación con QUALITY
    public class QualityBudgetItem : Entity
    {
        public Guid QualityId { get; set; }
        public Quality Quality { get; set; } = null!;
        public Guid BudgetItemId { get; set; }
        public BudgetItem BudgetItem { get; set; } = null!;
    }
    internal class QualityBudgetItemConfig : IEntityTypeConfiguration<QualityBudgetItem>
    {
        public void Configure(EntityTypeBuilder<QualityBudgetItem> builder)
        {
          
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder.HasKey(qb => new { qb.QualityId, qb.BudgetItemId });

            builder.HasOne(qb => qb.BudgetItem)
                .WithMany(b => b.QualityBudgetItems)
                .HasForeignKey(qb => qb.BudgetItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(qb => qb.Quality)
                .WithMany(x => x.QualityBudgetItems) // Si agregaste la colección en Quality pon: .WithMany(q => q.QualityBudgetItems)
                .HasForeignKey(qb => qb.QualityId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
