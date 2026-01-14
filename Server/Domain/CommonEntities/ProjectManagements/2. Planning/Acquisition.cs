namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Acquisition : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }



        public string Name { set; get; } = string.Empty;

        


    }
    internal class AcquisitionConfig : IEntityTypeConfiguration<Acquisition>
    {
        public void Configure(EntityTypeBuilder<Acquisition> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }

}
