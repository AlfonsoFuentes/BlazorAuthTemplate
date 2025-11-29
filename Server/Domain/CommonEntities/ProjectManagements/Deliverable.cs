namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Deliverable : Entity
    {
        
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
  
        public List<NewGanttTask> NewGanttTasks { get; set; } = new();
        public string? DurationUnit { get; set; } = string.Empty;
        public double DurationInDays { get; set; } = 0;
        public double DurationInUnit { get; set; } = 0;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MainOrder {  get; set; }
        public int InternalOrder {  get; set; }
        public string WBS => $"{InternalOrder}";
        public static Deliverable Create(Guid projectId, int order)
        {
            return new()
            {
                ProjectId = projectId,
                Order = order,
            };
        }
  
    }

}
