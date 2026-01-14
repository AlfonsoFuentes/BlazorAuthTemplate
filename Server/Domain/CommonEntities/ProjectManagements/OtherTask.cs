namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class OtherTask : Entity
    {
        //

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string Name { set; get; } = string.Empty;
       

      

    }
    internal class OtherTaskConfig : IEntityTypeConfiguration<OtherTask>
    {
        public void Configure(EntityTypeBuilder<OtherTask> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }

}
