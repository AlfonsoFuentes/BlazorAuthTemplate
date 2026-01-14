

namespace Server.Domain.CommonEntities.BudgetItems
{
   
    public class BudgetItemGanttTask : Entity
    {
        public Guid GanttTaskId { get; set; }
        public GanttTask GanttTask { get; set; } = null!;
        public Guid BudgetItemId { get; set; }
        public BudgetItem BudgetItem { get; set; } = null!;
        public decimal AmountAssigned { get; set; }
    }
    internal class BudgetItemGanttTaskConfig : IEntityTypeConfiguration<BudgetItemGanttTask>
    {
        public void Configure(EntityTypeBuilder<BudgetItemGanttTask> builder)
        {
            // ... (Tus configuraciones de Key y QueryFilter quedan igual) ...
            builder.HasKey(bg => new { bg.GanttTaskId, bg.BudgetItemId });
            builder.HasQueryFilter(x => x.IsDeleted == false);

            // Relación 1: Mantenemos Cascade (Por ejemplo, si borras el BudgetItem, se borra la relación)
            builder.HasOne(bg => bg.BudgetItem)
                .WithMany(b => b.BudgetItemGanttTasks)
                .HasForeignKey(bg => bg.BudgetItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⚠️ CORRECCIÓN AQUÍ:
            // Relación 2: Cambiamos a RESTRICT para romper el ciclo en la BD
            builder.HasOne(bg => bg.GanttTask)
                .WithMany(g => g.BudgetItemGanttTasks)
                .HasForeignKey(bg => bg.GanttTaskId)
                .OnDelete(DeleteBehavior.Restrict); // <-- Esto soluciona el error
        }
    }
}
