namespace Shared.Enums.ProjectNeedTypes
{
    public class ProjectStatusEnum : ValueObject
    {
        public const int NONE_ID = -1;
        public const int CREATED_ID = 0;
        public const int PLANNING_ID = 1;
        public const int EXECUTION_ID = 2;
        public const int CLOSED_ID = 3;
        public const int DISCARTED_ID = 4;
        public override string ToString()
        {
            return Name;
        }
        public static ProjectStatusEnum None = Create(NONE_ID, "None");
        public static ProjectStatusEnum Created = Create(CREATED_ID, "Created");
        public static ProjectStatusEnum Planning = Create(PLANNING_ID, "Planning");
        public static ProjectStatusEnum Execution = Create(EXECUTION_ID, "On Execution");
        public static ProjectStatusEnum Closed = Create(CLOSED_ID, "Closed");
        public static ProjectStatusEnum Discarted = Create(DISCARTED_ID, "Discarted");


        public static List<ProjectStatusEnum> List = new List<ProjectStatusEnum>()
            {
            None, Created, Planning, Execution, Closed,Discarted    
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static ProjectStatusEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
        public static ProjectStatusEnum Create(int id, string name) => new ProjectStatusEnum() { Id = id, Name = name };
        public static ProjectStatusEnum GetType(string name) => List.Exists(x => x.Name == name) ? List.FirstOrDefault(x => x.Name == name)!
            : None;
    }
}
