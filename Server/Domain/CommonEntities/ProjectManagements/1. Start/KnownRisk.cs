using Server.Domain.CommonEntities.BudgetItems;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class KnownRisk : Entity
    {
        
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
       
        public ICollection<KnownRiskBudgetItem> KnownRiskBudgetItems { get; set; } = new List<KnownRiskBudgetItem>();

        [NotMapped]
        public List<BudgetItem> RelatedBudgetItems => KnownRiskBudgetItems?.Select(x => x.BudgetItem).ToList() ?? new();


    }
    internal class KnownRiskConfig : IEntityTypeConfiguration<KnownRisk>
    {
        public void Configure(EntityTypeBuilder<KnownRisk> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
