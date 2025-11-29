namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class BackGround : Entity
    {
        //

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public static BackGround Create(Guid ProjectId, int Order)
        {
            return new BackGround()
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                Order = Order,

            };
        }

    }
}
