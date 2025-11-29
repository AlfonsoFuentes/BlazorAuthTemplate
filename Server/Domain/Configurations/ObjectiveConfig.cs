namespace Server.Domain.Configurations
{
    internal class ObjectiveConfig : IEntityTypeConfiguration<Objective>
    {
        public void Configure(EntityTypeBuilder<Objective> builder)
        {
            builder.HasKey(ci => ci.Id);

           
     

        }

    }
}
