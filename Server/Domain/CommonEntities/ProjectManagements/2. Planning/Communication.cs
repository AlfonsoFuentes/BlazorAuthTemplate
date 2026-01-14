using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Communication : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }



       
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty; // Ej: "Reporte de Avance Fase 1"

        public ActionCategory Type { get; set; } // Push, Pull, Interactive
        public ArtifactType Artifact { get; set; }  // Reporte, Reunión...

        // --- LÓGICA DEL TRIGGER (VINCULACIÓN GANTT) ---

        public CommunicationTrigger Trigger { get; set; }

        // Si Trigger es TaskStart, TaskEnd o WhileTaskActive, usamos esto:
        public Guid? LinkedGanttTaskId { get; set; }
        public virtual GanttTask? LinkedGanttTask { get; set; }

        // --- LÓGICA DE TIEMPO (OFFSET / FRECUENCIA) ---

        // Si Trigger == Periodic: Cada cuántos días (Ej: 7 = Semanal).
        // Si Trigger == TaskStart/End: Cuántos días desfazar (Ej: -5 = 5 días antes, +2 = 2 días después).
        // Si Trigger == WhileTaskActive: Cada cuántos días mientras dure la tarea.
        public int DaysOffsetOrFrequency { get; set; }

        // --- DESTINATARIOS (AUDIENCIA) ---
        // Relación muchos a muchos con los interesados del proyecto
        public virtual ICollection<StakeHolder> Receivers { get; set; } = new List<StakeHolder>();



    }
    internal class CommunicationConfig : IEntityTypeConfiguration<Communication>
    {
        public void Configure(EntityTypeBuilder<Communication> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder
               .HasMany(c => c.Receivers)
               .WithMany(s => s.Communications)
               .UsingEntity(j => j.ToTable("CommunicationReceivers")); // Tabla puente automática

            builder.HasOne(c => c.LinkedGanttTask)      // Una Comunicación tiene una Tarea
             .WithMany(t => t.Communications)     // Una Tarea tiene muchas Comunicaciones
             .HasForeignKey(c => c.LinkedGanttTaskId) // La FK es LinkedGanttTaskId
             .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
