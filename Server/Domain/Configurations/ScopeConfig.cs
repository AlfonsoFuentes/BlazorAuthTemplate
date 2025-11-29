namespace Server.Domain.Configurations
{
    internal class ScopeConfig : IEntityTypeConfiguration<Scope>
    {
        public void Configure(EntityTypeBuilder<Scope> builder)
        {
            builder.HasKey(ci => ci.Id);

      
        }

    }
}
