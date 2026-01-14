namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class RoleInsideProject : Entity
    {
        public string Name { get; set; } = string.Empty;
        [ForeignKey("RoleInsideProjectId")]
        public List<StakeHolder> StakeHolders { get; set; } = new();
        public Guid ProjectId { get; set; }

        public static RoleInsideProject Create(Guid _projectId, string Name)
        {
            return new()
            {
                Id = Guid.NewGuid(),

                ProjectId = _projectId,
                Name = Name
            };
        }
    }
    internal class RoleInsideProjectConfig : IEntityTypeConfiguration<RoleInsideProject>
    {
        public void Configure(EntityTypeBuilder<RoleInsideProject> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
