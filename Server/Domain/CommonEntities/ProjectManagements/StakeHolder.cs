namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class StakeHolder : Entity
    {
        public List<Project> Projects { get; } = [];
    

        //[ForeignKey("StakeHolderId")]
        //public List<MeetingAttendant> MeetingAttendants { get; } = [];
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string Area { get; set; } = string.Empty;

        [ForeignKey("ExpertId")]
        public List<ExpertJudgement> Judgements { get; set; } = new();

        [ForeignKey("RequestedById")]
        public List<Requirement> RequirementRequestedBys { get; set; } = new();
        [ForeignKey("ResponsibleId")]
        public List<Requirement> RequirementResponsibles { get; set; } = new();
      
        
        public static StakeHolder Create()
        {
            return new()
            {
                Id = Guid.NewGuid(),

            };
        }
        public RoleInsideProject? RoleInsideProject { get; set; } = null!;
        public Guid? RoleInsideProjectId { get; set; }
    }
}
