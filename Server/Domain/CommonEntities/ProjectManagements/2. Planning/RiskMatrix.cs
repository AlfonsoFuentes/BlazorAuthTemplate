using Server.Domain.CommonEntities.BudgetItems;
using Shared.Dtos.Plannings.RiskMatrixs;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class RiskMatrix : Entity
    {
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty; // Nuevo
        public string Cause { get; set; } = string.Empty;
        public string RiskEvent { get; set; } = string.Empty; // Nuevo
        public string Effect { get; set; } = string.Empty;

        // --- PMP Analysis (Usamos los Enums directamente) ---
        // EF Core los guardará como int (0, 1, 2...) por defecto
        public RiskProbability Probability { get; set; }
        public RiskImpact Impact { get; set; }

        // --- PMP Response ---
        public RiskStrategyType StrategyType { get; set; } // Nuevo (Avoid, Mitigate...)

        // Renombramos 'RiskMitigation' a 'ResponsePlanDescription' para ser más claros
        public string ResponsePlanDescription { get; set; } = string.Empty;

        public string Trigger { get; set; } = string.Empty; // Nuevo

        public RiskStatus Status { get; set; }
        public string Responsible { get; set; } = string.Empty;

        // --- Relaciones ---
        public List<RiskMatrixComment> RiskMatrixComments { get; set; } = new();

        // Tabla intermedia para inversiones (Testing, Equipment, etc.)
        public ICollection<RiskBudgetItem> RiskBudgetItems { get; set; } = new List<RiskBudgetItem>();

        // --- Helpers [NotMapped] ---
        [NotMapped]
        public int RiskScore => (int)Probability * (int)Impact;

        [NotMapped]
        public List<BudgetItem> MitigatingItems => RiskBudgetItems?.Select(x => x.BudgetItem).ToList() ?? new();
        public virtual ICollection<RiskResponseAction> RiskResponseActions { get; set; } = new List<RiskResponseAction>();
    }
    internal class RiskMatrixConfig : IEntityTypeConfiguration<RiskMatrix>
    {
        public void Configure(EntityTypeBuilder<RiskMatrix> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);
            builder
                .HasMany(x => x.RiskMatrixComments)
                  .WithOne(t => t.RiskMatrix)
                  .HasForeignKey(e => e.RiskMatrixId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.RiskResponseActions)
             .WithOne(t => t.RiskMatrix)
             .HasForeignKey(e => e.RiskMatrixId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
