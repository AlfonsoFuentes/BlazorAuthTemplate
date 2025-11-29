namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Assumption : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }



        public string Name { set; get; } = string.Empty;

        public static Assumption Create(Guid ProjectId,  int Order)
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
