namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class ExpertJudgement : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { set; get; } = string.Empty;

        public StakeHolder? Expert { get; set; }
        public Guid? ExpertId { get; set; }
      


    }
    internal class ExpertJudgementConfig : IEntityTypeConfiguration<ExpertJudgement>
    {
        public void Configure(EntityTypeBuilder<ExpertJudgement> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder.HasOne(c => c.Expert)
          .WithMany(t => t.Judgements)
          .HasForeignKey(x => x.ExpertId)
          .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
