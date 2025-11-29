namespace Shared.Enums.DiameterEnum
{
    public class NominalDiameterEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Name}";
        }

        public static NominalDiameterEnum Create(int id, string name) => new NominalDiameterEnum() { Id = id, Name = name };
        public static NominalDiameterEnum None = Create(-1, "None");
        public static NominalDiameterEnum Dia_0_5 = Create(0, "1/2 ''");
        public static NominalDiameterEnum Dia_3_4 = Create(1, "3/4 ''");
        public static NominalDiameterEnum Dia_1 = Create(2, "1 ''");
        public static NominalDiameterEnum Dia_1_1_4 = Create(3, "1 1/4 ''");
        public static NominalDiameterEnum Dia_1_5 = Create(4, "1 1/2 ''");
        public static NominalDiameterEnum Dia_2 = Create(5, "2 ''");
        public static NominalDiameterEnum Dia_2_5 = Create(6, "2 1/2 ''");
        public static NominalDiameterEnum Dia_3 = Create(7, "3 ''");
        public static NominalDiameterEnum Dia_4 = Create(8, "4 ''");
        public static NominalDiameterEnum Dia_6 = Create(9, "6 ''");
        public static NominalDiameterEnum Dia_8 = Create(10, "8 ''");
        public static NominalDiameterEnum Dia_10 = Create(11, "10 ''");
        public static NominalDiameterEnum NotApplicable = Create(12, "NA");
        public static List<NominalDiameterEnum> List = new List<NominalDiameterEnum>()
        {
            None, Dia_0_5,Dia_3_4,Dia_1,Dia_1_1_4,Dia_1_5,Dia_2,Dia_2_5,Dia_3,Dia_4,Dia_6,Dia_8,Dia_10,NotApplicable

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static NominalDiameterEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;

        public static NominalDiameterEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}