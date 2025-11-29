namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Bennefit : Entity
    {
        
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }


        public string Name { get; set; } = string.Empty;
        public static Bennefit Create(Guid ProjectId,  int Order)
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
