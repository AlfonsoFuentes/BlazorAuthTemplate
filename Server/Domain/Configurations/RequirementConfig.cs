namespace Server.Domain.Configurations
{
    internal class RequirementConfig : IEntityTypeConfiguration<Requirement>
    {
        public void Configure(EntityTypeBuilder<Requirement> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasOne(c => c.RequestedBy)
       .WithMany(t => t.RequirementRequestedBys)
       .HasForeignKey(x => x.RequestedById)
       .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Responsible)
       .WithMany(t => t.RequirementResponsibles)
       .HasForeignKey(x => x.ResponsibleId)
       .OnDelete(DeleteBehavior.NoAction);

         

        }

    }
}
