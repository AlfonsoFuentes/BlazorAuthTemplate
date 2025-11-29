namespace Shared.Enums.NozzleTypes
{
    public class NozzleTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Name}";
        }

        public static NozzleTypeEnum Create(int id, string name) => new NozzleTypeEnum() { Id = id, Name = name };
        public static NozzleTypeEnum None = Create(-1, "None");
        public static NozzleTypeEnum Inlet = Create(0, "Inlet");
        public static NozzleTypeEnum Outlet = Create(1, "Outlet");

        public static List<NozzleTypeEnum> List = new List<NozzleTypeEnum>()
        {
              None, Inlet, Outlet

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static NozzleTypeEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;

        public static NozzleTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}