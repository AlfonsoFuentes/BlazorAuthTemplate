namespace Shared.Enums.DiameterEnum
{
    public class PipeClassEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Name}";
        }

        public static PipeClassEnum Create(int id, string name) => new PipeClassEnum() { Id = id, Name = name };

        public static PipeClassEnum None = Create(-1, "None");
        public static PipeClassEnum Sanitary3A = Create(0, "Sanitary 3A");
        public static PipeClassEnum SCH10 = Create(1, "SCH 10");
        public static PipeClassEnum SCH40 = Create(1, "SCH 40");
        public static PipeClassEnum SCH80 = Create(2, "SCH 80");


        public static List<PipeClassEnum> List = new List<PipeClassEnum>()
        {
            None, Sanitary3A,SCH10,SCH40,SCH80

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static PipeClassEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;

        public static PipeClassEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}