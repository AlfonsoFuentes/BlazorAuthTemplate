namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class RiskMatrixComment : Entity
    {
        public RiskMatrix RiskMatrix { get; set; } = null!;
        public Guid RiskMatrixId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CommentDate { get; set; }
        public string CommentedBy { get; set; } = string.Empty;
    }
    internal class RiskMatrixCommentConfig : IEntityTypeConfiguration<RiskMatrixComment>
    {
        public void Configure(EntityTypeBuilder<RiskMatrixComment> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
