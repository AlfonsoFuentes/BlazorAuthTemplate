using Shared.Enums.Materials;

namespace Shared.Enums.ValvesEnum
{
    public class ValveTypesEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }
        public string Letter { get; set; } = string.Empty;
        public static ValveTypesEnum Create(int id, string letter, string name) => new ValveTypesEnum() { Id = id, Letter = letter, Name = name };
        public static ValveTypesEnum None = Create(-1, "N", "None");
        public static ValveTypesEnum Butterfly = Create(0, "VBU", "Butterfly");
        public static ValveTypesEnum Ball = Create(1, "VBA", "Ball");
        public static ValveTypesEnum Diaphragm = Create(2, "VDI", "Diaphragm");
        public static ValveTypesEnum Gate = Create(3, "VGA", "Gate");
        public static ValveTypesEnum Globe = Create(4, "VGL", "Globe");
        public static ValveTypesEnum Knife = Create(5, "VKN", "Knife");
        public static ValveTypesEnum Check = Create(6, "VCH", "Check");
        public static ValveTypesEnum Ball_Three_Way_T = Create(7, "VTVT", "T-Ball Three way");
        public static ValveTypesEnum Ball_Three_Way_L = Create(8, "VTVL", "L-Ball Three way");
        public static ValveTypesEnum Ball_Zero_deadLeg = Create(9, "BZDL", "Ball Zero dead leag");
        public static ValveTypesEnum Diaphragm_Zero_deadLeg = Create(10, "DZDL", "Diaphragm Zero dead leag");
        public static ValveTypesEnum Ball_Four_Way = Create(11, "VFV", "Ball Four way");
        public static ValveTypesEnum Other = Create(12, "VOT", "Other");
        public static ValveTypesEnum Sample_port = Create(13, "SP", "Sample port");
        public static List<ValveTypesEnum> List = new List<ValveTypesEnum>()
        {
            None,Butterfly, Ball, Diaphragm, Gate,Globe,Sample_port,Knife,Check,Ball_Three_Way_T, Ball_Three_Way_L,Ball_Zero_deadLeg,Diaphragm_Zero_deadLeg,Ball_Four_Way,Other

        };
        public static List<ValveTypesEnum> ControlledList = new List<ValveTypesEnum>()
        {
            None,Butterfly, Ball, Diaphragm,Globe

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static string GetLetter(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Letter : string.Empty;
        public static ValveTypesEnum GetTypeByName(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static ValveTypesEnum GetTypeByLetter(string letter) => List.Exists(x => x.Letter.Equals(letter)) ? List.FirstOrDefault(x => x.Letter.Equals(letter))! : None;
        public static ValveTypesEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}
