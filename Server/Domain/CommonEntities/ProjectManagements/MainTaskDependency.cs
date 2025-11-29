namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class MainTaskDependency : Entity
    {

        
        public NewGanttTask MainTask { get; set; } = null!;
        public Guid MainTaskId { get; set; }
        public NewGanttTask DependencyTask { get; set; } = null!;
        public Guid DependencyTaskId { get; set; }
        public int DependencyType { get; set; }
        public string? LagUnit { get; set; } = string.Empty;
        public double LagInDays { get; set; } = 0;
        public double LagInUnits { get; set; } = 0;
        public static MainTaskDependency Create(Guid _MainTaskId, Guid _DependecyTaskId)
        {
            return new MainTaskDependency()
            {
                Id = Guid.NewGuid(),
                MainTaskId = _MainTaskId,
                DependencyTaskId = _DependecyTaskId,



            };
        }

    }
}
