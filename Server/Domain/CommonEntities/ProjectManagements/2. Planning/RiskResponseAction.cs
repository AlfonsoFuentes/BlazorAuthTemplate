using Shared.Dtos.Plannings.RiskMatrixs;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class RiskResponseAction : Entity // Asumo que heredas de una base con Id
    {
        // Relación con el Riesgo Padre
        public Guid RiskMatrixId { get; set; }
        public virtual RiskMatrix RiskMatrix { get; set; } = null!;

        // Las 3 W del PMP (What, Who, When) + Estado
        public string Description { get; set; } = string.Empty; // What
        public string AssignedTo { get; set; } = string.Empty;  // Who
        public DateTime? DueDate { get; set; }                  // When
        public bool IsCompleted { get; set; } = false;          // Status

        // Tipo de plan al que pertenece este paso
        public RiskActionType ActionType { get; set; }
    }
    internal class RiskResponseActionConfig : IEntityTypeConfiguration<RiskResponseAction>
    {
        public void Configure(EntityTypeBuilder<RiskResponseAction> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
