namespace Shared.Enums.OtherTasks
{
    public class OtherTaskStatusEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static OtherTaskStatusEnum Create(int id, string name) => new OtherTaskStatusEnum() { Id = id, Name = name };


        public static OtherTaskStatusEnum Open = Create(0, "Open");

        public static OtherTaskStatusEnum Closed = Create(1, "Closed");

        public static List<OtherTaskStatusEnum> List = new List<OtherTaskStatusEnum>()
            {
        Open, Closed
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static OtherTaskStatusEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : Open;
        public static OtherTaskStatusEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : Open;
    }
}
