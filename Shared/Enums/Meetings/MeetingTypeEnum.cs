namespace Shared.Enums.Meetings
{
    public class MeetingTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }
        public static MeetingTypeEnum None = Create(-1, "None");
        public static MeetingTypeEnum ProjectStart = Create(0, "Project Start");
        public static MeetingTypeEnum Interview = Create(1, "Interview StakeHolder");
        public static MeetingTypeEnum Following = Create(2, "Project Following");
        public static MeetingTypeEnum Approval = Create(3, "Approval Project");



        public static List<MeetingTypeEnum> List = new List<MeetingTypeEnum>()
            {
            None, ProjectStart, Interview, Following, Approval
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static MeetingTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
        public static MeetingTypeEnum Create(int id, string name) => new MeetingTypeEnum() { Id = id, Name = name };
        public static MeetingTypeEnum GetType(string name) => List.Exists(x => x.Name == name) ? List.FirstOrDefault(x => x.Name == name)!
            : None;
    }
}
