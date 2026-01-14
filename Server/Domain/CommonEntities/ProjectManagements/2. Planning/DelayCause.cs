using System.ComponentModel.DataAnnotations;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class DelayCause : Entity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // "Proveedor", "Clima"

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(7)]
        public string ColorHex { get; set; } = "#808080"; // Hex code para KPIs

        public bool IsSystemDefault { get; set; } = false;

        // Relación inversa para saber dónde se usó
        public virtual ICollection<TaskChangeLog> RelatedLogs { get; set; } = new List<TaskChangeLog>();
    }
    internal class DelayCauseConfig : IEntityTypeConfiguration<DelayCause>
    {
        public void Configure(EntityTypeBuilder<DelayCause> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }

}
