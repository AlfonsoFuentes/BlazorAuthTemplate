using Shared.Dtos.Projects.Plannings.Gantts;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    // ─── Dependencia entre tareas (tabla intermedia)
    public class GanttDependency : Entity
    {
        // Tarea que *depende* (la "hija" lógica)

        public Guid TaskId { get; set; }
        public virtual GanttTask Task { get; set; } = null!;

        // Tarea *predecesora* (la que debe completarse/arrancar antes)
        public Guid PredecessorId { get; set; }
        public virtual GanttTask Predecessor { get; set; } = null!;

        // Tipo y lag
        public DependencyType Type { get; set; }
        public string Lag { get; set; } = "0d";
    }
    internal class GanttDependencyConfig : IEntityTypeConfiguration<GanttDependency>
    {
        public void Configure(EntityTypeBuilder<GanttDependency> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);
            builder.Property(d => d.Lag)
                  .IsRequired()
                  .HasMaxLength(32)
                  .HasDefaultValue("0d");

            // Relación: tarea que depende
            builder.HasOne(d => d.Task)
                   .WithMany(t => t.Dependencies)
                   .HasForeignKey(d => d.TaskId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            //// Relación: predecesora
            builder.HasOne(d => d.Predecessor)
                   .WithMany(t => t.Predecessors)
                   .HasForeignKey(d => d.PredecessorId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); // no borrar predecesora si hay dependencias

            // Índices
            builder.HasIndex(d => d.TaskId);
            builder.HasIndex(d => d.PredecessorId);
            builder.HasIndex(d => new { d.TaskId, d.PredecessorId }).IsUnique(); // evita duplicados

        }

    }
}
