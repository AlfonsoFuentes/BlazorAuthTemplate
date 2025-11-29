namespace Shared.Enums.ValvesEnum
{
    public class SignalTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static SignalTypeEnum Create(int id, string name) => new SignalTypeEnum() { Id = id, Name = name };

        public static SignalTypeEnum None = Create(-1, "None");
        public static SignalTypeEnum mA_4_20 = Create(0, "4 - 20 mA");
        public static SignalTypeEnum VDC24 = Create(1, "24 VDC");
        public static SignalTypeEnum VAC110 = Create(2, "110 VAC");
        public static SignalTypeEnum Ethernet = Create(3, "Ethernet");
        public static SignalTypeEnum ModBus = Create(4, "Mod Bus");
        public static SignalTypeEnum DeviceNet = Create(5, "Device Net");
        public static SignalTypeEnum Hart = Create(6, "HART");
        public static SignalTypeEnum NotApplicable = Create(7, "Not Applicable");
        public static List<SignalTypeEnum> List = new List<SignalTypeEnum>()
        {
            None,mA_4_20, VDC24, VAC110,Ethernet, ModBus, DeviceNet, Hart, NotApplicable
        };
      
        public static List<SignalTypeEnum> ListForValvesONOFF = new List<SignalTypeEnum>()
        {
            None, VDC24, VAC110
        };
        public static List<SignalTypeEnum> ListForValvesProportional = new List<SignalTypeEnum>()
        {
            None, mA_4_20
        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static SignalTypeEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static SignalTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
    }
}
