namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class LearnedLesson : Entity
    {
        public string Name { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public static LearnedLesson Create(Guid ProjectId,  int Order)
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
