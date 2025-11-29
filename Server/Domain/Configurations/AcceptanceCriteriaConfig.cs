

namespace Server.Domain.Configurations
{
    internal class AcceptanceCriteriaConfig : IEntityTypeConfiguration<AcceptanceCriteria>
    {
        public void Configure(EntityTypeBuilder<AcceptanceCriteria> builder)
        {
            builder.HasKey(ci => ci.Id);



        }

    }
}
