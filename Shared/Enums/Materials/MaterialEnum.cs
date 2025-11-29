namespace Shared.Enums.Materials
{
    public class MaterialEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static MaterialEnum Create(int id, string name) => new MaterialEnum() { Id = id, Name = name };

        public static MaterialEnum None = Create(-1, "None");
        public static MaterialEnum SS316L = Create(0, "SS316L");
        public static MaterialEnum SS304 = Create(1, "SS304");
        public static MaterialEnum SS205 = Create(2, "SS205");
        public static MaterialEnum CS = Create(3, "CS");
        public static MaterialEnum PVC = Create(4, "PVC");
        public static MaterialEnum CPVC = Create(5, "CPVC");
        public static MaterialEnum NotApplicable = Create(6, "NA");
        public static MaterialEnum PP = Create(7, "Polipropilene");
        public static List<MaterialEnum> List = new List<MaterialEnum>()
            {
          None, SS316L, SS304, SS205,CS, PVC, CPVC,PP,NotApplicable
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static MaterialEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static MaterialEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
    }
}
