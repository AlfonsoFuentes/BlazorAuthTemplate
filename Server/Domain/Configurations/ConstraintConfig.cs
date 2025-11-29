namespace Server.Domain.Configurations
{
    internal class ConstraintConfig : IEntityTypeConfiguration<Constrainst>
    {
        public void Configure(EntityTypeBuilder<Constrainst> builder)
        {
            builder.HasKey(ci => ci.Id);

        

        }

    }
}
