namespace Shared.Enums.ValvesEnum
{
    public class ActuatorTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static ActuatorTypeEnum Create(int id, string name) => new ActuatorTypeEnum() { Id = id, Name = name };

        public static ActuatorTypeEnum None = Create(-1, "None");
        public static ActuatorTypeEnum Simple_effect = Create(0, "Simple effect");
        public static ActuatorTypeEnum Double_effect = Create(1, "Double effect");
        public static ActuatorTypeEnum Hand = Create(2, "Hand");
        public static ActuatorTypeEnum Not_Applicable = Create(3, "Not Applicable");

        public static List<ActuatorTypeEnum> List = new List<ActuatorTypeEnum>()
            {
           None,  Simple_effect, Double_effect, Hand,Not_Applicable,
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static ActuatorTypeEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static ActuatorTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
    }
}
