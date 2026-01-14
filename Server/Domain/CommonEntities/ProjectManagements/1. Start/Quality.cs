using Server.Domain.CommonEntities.BudgetItems;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Quality : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;


        public ICollection<QualityBudgetItem> QualityBudgetItems { get; set; } = new List<QualityBudgetItem>();

        [NotMapped]
        public List<BudgetItem> RelatedBudgetItems => QualityBudgetItems?.Select(x => x.BudgetItem).ToList() ?? new();


    }
    internal class QualityConfig : IEntityTypeConfiguration<Quality>
    {
        public void Configure(EntityTypeBuilder<Quality> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
