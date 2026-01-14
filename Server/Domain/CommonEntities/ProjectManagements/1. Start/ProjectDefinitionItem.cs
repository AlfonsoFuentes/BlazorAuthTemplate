using Shared.Enums.ProjectDefinitionTypes;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class ProjectDefinitionItem : Entity
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // Aquí definimos qué es (Objetivo, Alcance, etc.)
        public ProjectDefinitionType Type { get; set; }

        public string Name { get; set; } = string.Empty;

        // Agrego Description y Order porque siempre son útiles en listas UI
        public string? Description { get; set; }

    }

    internal class ProjectDefinitionItemConfig : IEntityTypeConfiguration<ProjectDefinitionItem>
    {
        public void Configure(EntityTypeBuilder<ProjectDefinitionItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            // Índice compuesto para busquedas rápidas: "Dame todos los Objetivos del Proyecto X"
            builder.HasIndex(x => new { x.ProjectId, x.Type });

            builder.HasOne(x => x.Project)
                .WithMany(p => p.DefinitionItems) // Ahora vamos a Project a cambiar esto
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    //public class Bennefit : Entity
    //{

    //    public Project Project { get; set; } = null!;
    //    public Guid ProjectId { get; set; }


    //    public string Name { get; set; } = string.Empty;



    //}
    //internal class BennefitConfig : IEntityTypeConfiguration<Bennefit>
    //{
    //    public void Configure(EntityTypeBuilder<Bennefit> builder)
    //    {
    //        builder.HasKey(ci => ci.Id);
    //        builder.HasQueryFilter(x => x.IsDeleted == false);


    //    }

    //}
}
