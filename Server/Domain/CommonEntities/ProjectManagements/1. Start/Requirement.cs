using Shared.Enums.RequirementPrioritys;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Requirement : Entity
    {


        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }

        public string Name { set; get; } = string.Empty;
        public string Type { set; get; } = string.Empty;
        public StakeHolder? RequestedBy { get; set; }
        public Guid? RequestedById { get; set; }
        public StakeHolder? Responsible { get; set; }
        public Guid? ResponsibleId { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { set; get; } = string.Empty;

        public RequirementPriorityEnum PriorityEnum => RequirementPriorityEnum.GetType(Priority);
        public RequirementTypeEnum TypeEnum => RequirementTypeEnum.GetType(Type);




    }
    internal class RequirementConfig : IEntityTypeConfiguration<Requirement>
    {
        public void Configure(EntityTypeBuilder<Requirement> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder.HasOne(c => c.RequestedBy)
      .WithMany(t => t.RequirementRequestedBys)
      .HasForeignKey(x => x.RequestedById)
      .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Responsible)
       .WithMany(t => t.RequirementResponsibles)
       .HasForeignKey(x => x.ResponsibleId)
       .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
