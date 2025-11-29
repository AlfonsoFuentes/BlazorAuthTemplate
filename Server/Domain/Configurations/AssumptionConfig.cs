namespace Server.Domain.Configurations
{
    internal class AssumptionConfig : IEntityTypeConfiguration<Assumption>
    {
        public void Configure(EntityTypeBuilder<Assumption> builder)
        {
            builder.HasKey(ci => ci.Id);

          

        }

    }
}
