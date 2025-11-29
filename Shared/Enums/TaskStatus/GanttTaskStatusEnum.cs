namespace Shared.Enums.TaskStatus
{
    public class GanttTaskStatusEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static GanttTaskStatusEnum Create(int id, string name) => new GanttTaskStatusEnum() { Id = id, Name = name };


        public static GanttTaskStatusEnum NotInitiated = Create(0, "Not iniated");
        public static GanttTaskStatusEnum OnGoing = Create(1, "On going");
        public static GanttTaskStatusEnum Closed = Create(2, "Closed");

        public static List<GanttTaskStatusEnum> List = new List<GanttTaskStatusEnum>()
            {
         NotInitiated, OnGoing, Closed
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static GanttTaskStatusEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : NotInitiated;
        public static GanttTaskStatusEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : NotInitiated;
    }
}
