

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class AcceptanceCriteria : Entity
    {
        
        public string Name { get; set; } = string.Empty;

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }


        public static AcceptanceCriteria Create(Guid ProjectId,  int Order)
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
