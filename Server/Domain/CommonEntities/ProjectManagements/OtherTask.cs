namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class OtherTask : Entity
    {
        //

        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        //public int Status { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string Name { set; get; } = string.Empty;
        [NotMapped]
        public string CECName => Project == null ? string.Empty : $"CEC0000{Project.ProjectNumber}";
        [NotMapped]
        public string ProjectName => Project == null ? string.Empty : Project.Name;

        public static OtherTask Create(Guid ProjectId, int Order)
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
