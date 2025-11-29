namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class ExpertJudgement : Entity
    {
        

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { set; get; } = string.Empty;

        public StakeHolder? Expert { get; set; }
        public Guid? ExpertId { get; set; }
        public static ExpertJudgement Create(Guid ProjectId,  int Order)
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
