namespace Server.Domain.Configurations
{
    internal class StakeHolderConfig : IEntityTypeConfiguration<StakeHolder>
    {
        public void Configure(EntityTypeBuilder<StakeHolder> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasOne(c => c.RoleInsideProject)
         .WithMany(t => t.StakeHolders)
         .HasForeignKey(x => x.RoleInsideProjectId)
         .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
