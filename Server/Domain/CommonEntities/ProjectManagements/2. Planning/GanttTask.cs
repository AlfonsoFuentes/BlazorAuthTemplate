using Server.Domain.CommonEntities.BudgetItems;
using Shared.Dtos.Projects.Plannings.Gantts;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    // ─── Tarea Gantt (entidad persistente)
    public class GanttTask : Entity
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Duration { get; set; } = "1d";
       
        public bool IsMilestone { get; set; }
        public string ResponsibleId { get; set; } = string.Empty;

        public GanttField? LastModifiedField { get; set; }


        public virtual Project Project { get; set; } = null!;
        public Guid? ParentId { get; set; }
        public virtual GanttTask? Parent { get; set; }

        public virtual ICollection<GanttDependency> Dependencies { get; set; } = new List<GanttDependency>();

        [ForeignKey("PredecessorId")]
        public virtual ICollection<GanttDependency> Predecessors { get; set; } = new List<GanttDependency>();
        public virtual ICollection<Communication> Communications { get; set; } = new List<Communication>();

        public ICollection<BudgetItemGanttTask> BudgetItemGanttTasks { get; set; } = new List<BudgetItemGanttTask>();

        [NotMapped]
        public decimal TotalCost => BudgetItemGanttTasks?.Sum(x => x.BudgetItem.BudgetUSD) ?? 0;
    }
    internal class GanttTaskConfig : IEntityTypeConfiguration<GanttTask>
    {
        public void Configure(EntityTypeBuilder<GanttTask> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(t => t.Duration)
                   .IsRequired()
                   .HasMaxLength(32)
                   .HasDefaultValue("1d");

            builder.Property(t => t.ResponsibleId)
                   .HasMaxLength(128);

            //builder.Property(t => t.Phase)
            //       .HasMaxLength(128);

            // Relación con Project
            builder.HasOne(t => t.Project)
                   .WithMany(p => p.GanttTasks)
                   .HasForeignKey(t => t.ProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // Jerarquía: auto-referencia
            builder.HasOne(t => t.Parent)
             .WithMany()  // ← sin parámetro: relación unidireccional
             .HasForeignKey(t => t.ParentId)
             .OnDelete(DeleteBehavior.Restrict);

            // Índices para rendimiento
            builder.HasIndex(t => t.ProjectId);
            builder.HasIndex(t => t.ParentId);
            builder.HasIndex(t => t.Order);
            builder.HasIndex(t => new { t.ProjectId, t.ParentId });

        }

    }


}
