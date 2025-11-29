using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Server.Domain.Configurations
{
    internal class NewGanttTaskConfig : IEntityTypeConfiguration<NewGanttTask>
    {
        public void Configure(EntityTypeBuilder<NewGanttTask> builder)
        {
            builder.HasKey(ci => ci.Id);

            // Configurar la relación padre-hijo
            builder
                .HasOne(m => m.Parent) // Un hito tiene un padre
                .WithMany(m => m.SubTasks) // Un padre puede tener muchos subhitos
                .HasForeignKey(m => m.ParentId) // Clave foránea
                .OnDelete(DeleteBehavior.Restrict); // Evita la eliminación en cascada para evitar problemas


           
        }

    }
}
