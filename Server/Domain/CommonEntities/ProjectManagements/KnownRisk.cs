namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class KnownRisk : Entity
    {
        
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public static KnownRisk Create(Guid ProjectId,  int Order)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                Order = Order,
               
               
            };
        }

       
        
    }
}
