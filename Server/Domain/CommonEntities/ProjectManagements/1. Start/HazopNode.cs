using System.ComponentModel.DataAnnotations;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class HazopNode : Entity
    {

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
     
        public string Name { get; set; } = string.Empty; // Ej: "Node 1: Suction Line"

        public string Description { get; set; } = string.Empty;
        public string DesignIntent { get; set; } = string.Empty; // Intención del diseño original

        // Relación 1:N con las desviaciones
        public virtual ICollection<HazopDetail> Details { get; set; } = new List<HazopDetail>();
    }
    internal class HazopNodeConfig : IEntityTypeConfiguration<HazopNode>
    {
        public void Configure(EntityTypeBuilder<HazopNode> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder.HasMany(x => x.Details)
          .WithOne(t => t.HazopNode)
          .HasForeignKey(e => e.HazopNodeId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
