namespace Shared.Enums.ValvesEnum
{
    public class FailTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static FailTypeEnum Create(int id, string name) => new FailTypeEnum() { Id = id, Name = name };

        public static FailTypeEnum None = Create(-1, "None");
        public static FailTypeEnum Open = Create(0, "Open - Air to close");
        public static FailTypeEnum Closed = Create(1, "Close - Air to open");

        public static FailTypeEnum Not_Applicable = Create(2, "Not Applicable");


        public static List<FailTypeEnum> List = new List<FailTypeEnum>()
            {
       None, Open, Closed, Not_Applicable
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static FailTypeEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static FailTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
    }
}
