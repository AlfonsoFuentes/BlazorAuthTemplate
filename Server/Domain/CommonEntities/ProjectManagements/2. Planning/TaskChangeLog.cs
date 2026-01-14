using System.ComponentModel.DataAnnotations;

namespace Server.Domain.CommonEntities.ProjectManagements
{

    public class TaskChangeLog : Entity
    {
        public Guid GanttTaskId { get; set; }
        public virtual GanttTask GanttTask { get; set; } = null!;

        // Auditoría del cambio
        [MaxLength(50)]
        public string FieldChanged { get; set; } = string.Empty; // "EndDate"
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;

        // ✅ La Razón del Cambio (Estandarizada)
        public Guid DelayCauseId { get; set; }
        public virtual DelayCause DelayCause { get; set; } = null!;

        // Comentario específico
        [MaxLength(1000)]
        public string SpecificComment { get; set; } = string.Empty;

        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
        public string ChangedByUserId { get; set; } = string.Empty;
    }
    internal class TaskChangeLogConfig : IEntityTypeConfiguration<TaskChangeLog>
    {
        public void Configure(EntityTypeBuilder<TaskChangeLog> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);
            builder
             .HasOne(x => x.GanttTask)
             .WithMany() // Asumiendo que no agregaste colección en GanttTask, sino unidireccional
             .HasForeignKey(x => x.GanttTaskId)
             .OnDelete(DeleteBehavior.Cascade);

            // Si borras una Causa Genérica, NO borres el log, ponlo en null o Restrict
            // (O mejor, evita borrar Causas que ya se usaron)
            builder
                .HasOne(x => x.DelayCause)
                .WithMany(x => x.RelatedLogs)
                .HasForeignKey(x => x.DelayCauseId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
