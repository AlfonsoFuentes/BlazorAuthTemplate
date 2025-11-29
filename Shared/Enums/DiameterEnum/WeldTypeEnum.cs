namespace Shared.Enums.DiameterEnum
{
    public class WeldTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Name}";
        }

        public static WeldTypeEnum Create(int id, string name) => new WeldTypeEnum() { Id = id, Name = name };
        public static WeldTypeEnum None = Create(-1, "None");

        public static WeldTypeEnum Orbital = Create(0, "Orbital");
        public static WeldTypeEnum Normal = Create(1, "Normal");

        public static List<WeldTypeEnum> List = new List<WeldTypeEnum>()
        {
            None,  Orbital,Normal,

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static WeldTypeEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;

        public static WeldTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}