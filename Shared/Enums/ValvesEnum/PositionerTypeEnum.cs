using Shared.Enums.Materials;

namespace Shared.Enums.ValvesEnum
{
    public class PositionerTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }
        public string Letter { get; set; } = string.Empty;
        public static PositionerTypeEnum Create(int id, string letter, string name) => new PositionerTypeEnum() { Id = id, Letter = letter, Name = name };

        public static PositionerTypeEnum None = Create(-1, "N", "None");
        public static PositionerTypeEnum ON_OFF = Create(0, "S", "ON OFF");
        public static PositionerTypeEnum Proportional = Create(1, "C", "Proportional");
        public static PositionerTypeEnum Not_Applicable = Create(2, "H", "Not Applicable");

        public static List<PositionerTypeEnum> List = new List<PositionerTypeEnum>()
            {
                 None, ON_OFF,Proportional,Not_Applicable
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static PositionerTypeEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static PositionerTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
        
    }
}
