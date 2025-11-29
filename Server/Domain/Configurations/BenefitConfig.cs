namespace Server.Domain.Configurations
{
    internal class BenefitConfig : IEntityTypeConfiguration<Bennefit>
    {
        public void Configure(EntityTypeBuilder<Bennefit> builder)
        {
            builder.HasKey(ci => ci.Id);



        }

    }
}
