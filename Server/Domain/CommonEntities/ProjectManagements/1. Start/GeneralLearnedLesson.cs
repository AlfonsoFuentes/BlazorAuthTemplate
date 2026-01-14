namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class GeneralLearnedLesson : Entity
    {
        public string Name { get; set; } = string.Empty;


       
        [ForeignKey("GeneralLearnedLessonId")]
        public ICollection<LearnedLesson> LearnedLessons { get; set; } = new List<LearnedLesson>();


    }
    internal class GeneralLearnedLessonConfig : IEntityTypeConfiguration<GeneralLearnedLesson>
    {
        public void Configure(EntityTypeBuilder<GeneralLearnedLesson> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
