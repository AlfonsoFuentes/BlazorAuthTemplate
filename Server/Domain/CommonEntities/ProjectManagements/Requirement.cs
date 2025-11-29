namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class Requirement : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }

        public string Name { set; get; } = string.Empty;
        public string Type { set; get; } = string.Empty;
        public StakeHolder? RequestedBy { get; set; }
        public Guid? RequestedById { get; set; }
        public StakeHolder? Responsible { get; set; }
        public Guid? ResponsibleId { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { set; get; } = string.Empty;
        public static Requirement Create(Guid ProjectId,  int Order)
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
