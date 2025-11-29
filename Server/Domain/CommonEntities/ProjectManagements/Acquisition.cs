namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Acquisition : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }



        public string Name { set; get; } = string.Empty;

        public static Acquisition Create(Guid ProjectId,  int Order)
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
