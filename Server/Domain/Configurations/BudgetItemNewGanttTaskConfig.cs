using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.CommonEntities.BudgetItems;


namespace Server.Domain.Configurations
{
    internal class BudgetItemNewGanttTaskConfig : IEntityTypeConfiguration<BudgetItemNewGanttTask>
    {
        public void Configure(EntityTypeBuilder<BudgetItemNewGanttTask> builder)
        {
            builder.HasKey(td => td.Id);

          

            // Relación: BudgetItem -> NewGanttTasks
            builder
                .HasOne(td => td.BudgetItem)
                .WithMany(t => t.BudgetItemNewGanttTasks) // Fix: Ensure the navigation property matches the type
                .HasForeignKey(td => td.BudgetItemId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

            // Relación: NewGanttTask -> BudgetItems
            builder
                .HasOne(td => td.NewGanttTask)
                .WithMany(t => t.BudgetItemNewGanttTasks) // Fix: Ensure the navigation property matches the type
                .HasForeignKey(td => td.NewGanttTaskId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

    
        }
    }
}
