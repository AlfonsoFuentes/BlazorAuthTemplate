namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class MonitoringLog : Entity
    {
        
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? InitialDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public string? ClosingText { get; set; } = string.Empty;
       


    }
    internal class MonitoringLogConfig : IEntityTypeConfiguration<MonitoringLog>
    {
        public void Configure(EntityTypeBuilder<MonitoringLog> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
