using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Server.Domain.Configurations
{
    internal class MainTaskDependencyConfig : IEntityTypeConfiguration<MainTaskDependency>
    {
        public void Configure(EntityTypeBuilder<MainTaskDependency> builder)
        {
            builder.HasKey(td => td.Id);



           
            builder
                .HasOne(td => td.MainTask)
                .WithMany(t => t.MainTasks) // Fix: Ensure the navigation property matches the type
                .HasForeignKey(td => td.MainTaskId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

         
            builder
                .HasOne(td => td.DependencyTask)
                .WithMany(t => t.DependencyTasks) // Fix: Ensure the navigation property matches the type
                .HasForeignKey(td => td.DependencyTaskId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada
        }
    }
}
