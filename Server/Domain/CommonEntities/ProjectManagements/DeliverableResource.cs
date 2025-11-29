namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class DeliverableResource : Entity
    {
        
        //public GanttTask GanttTask { get; set; } = null!;
        //public Guid GanttTaskId { get; set; }
        public Guid ResourceId { get; set; }
        public string Avalabilty { get; set; } = string.Empty;

    }
}
