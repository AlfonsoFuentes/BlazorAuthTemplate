namespace Server.Domain.Configurations
{
    internal class KnownRiskConfig : IEntityTypeConfiguration<KnownRisk>
    {
        public void Configure(EntityTypeBuilder<KnownRisk> builder)
        {
            builder.HasKey(ci => ci.Id);



        }

    }
}
