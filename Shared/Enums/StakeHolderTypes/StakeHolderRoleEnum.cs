namespace Shared.Enums.StakeHolderTypes
{
    public class StakeHolderRoleEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }
        public static StakeHolderRoleEnum None = Create(-1, "None");
        public static StakeHolderRoleEnum FunctionalDirector = Create(0, "Functional Director");
        public static StakeHolderRoleEnum TeamMember = Create(1, "Team member");
        public static StakeHolderRoleEnum Client = Create(2, "Client");
        public static StakeHolderRoleEnum FinalUser = Create(3, "Final User");
        public static StakeHolderRoleEnum Supplier = Create(4, "Supplier");
        public static StakeHolderRoleEnum Partner = Create(5, "Partner");
        public static StakeHolderRoleEnum OrganizationalGroup = Create(6, "Organizational Group");
        public static StakeHolderRoleEnum Expert = Create(7, "Expert");
        public static StakeHolderRoleEnum Other = Create(8, "Other");
        public static StakeHolderRoleEnum Manager = Create(9, "Manager");
        public static StakeHolderRoleEnum Sponsor = Create(10, "Sponsor");
        public static StakeHolderRoleEnum ProjectLeader = Create(11, "Project Leader");

        public static List<StakeHolderRoleEnum> List = new List<StakeHolderRoleEnum>()
            {
            None,FunctionalDirector, TeamMember, Client, FinalUser, Supplier, Partner, OrganizationalGroup,Expert,Manager,Sponsor,ProjectLeader, Other
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static StakeHolderRoleEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
        public static StakeHolderRoleEnum Create(int id, string name) => new StakeHolderRoleEnum() { Id = id, Name = name };
        public static StakeHolderRoleEnum GetType(string name) => List.Exists(x => x.Name == name) ? List.FirstOrDefault(x => x.Name == name)!
            : None;
    }

}
