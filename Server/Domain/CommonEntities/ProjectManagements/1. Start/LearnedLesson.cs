namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class LearnedLesson : Entity
    {
        public string Name { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public GeneralLearnedLesson? GeneralLearnedLesson { get; set; } = null!;
        public Guid? GeneralLearnedLessonId { get; set; }


    }
    internal class LearnedLessonConfig : IEntityTypeConfiguration<LearnedLesson>
    {
        public void Configure(EntityTypeBuilder<LearnedLesson> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder
                .HasOne(c => c.GeneralLearnedLesson)
                .WithMany(t => t.LearnedLessons)
                .HasForeignKey(x => x.GeneralLearnedLessonId)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
