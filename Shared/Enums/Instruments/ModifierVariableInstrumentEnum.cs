namespace Shared.Enums.Instruments
{
    public class ModifierVariableInstrumentEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Name}";
        }

        public string Letter { get; set; } = string.Empty;
        public static ModifierVariableInstrumentEnum Create(int id, string letter, string name) => new ModifierVariableInstrumentEnum() { Id = id, Letter = letter, Name = name };
        public static ModifierVariableInstrumentEnum None = Create(-1, string.Empty, "None");
        public static ModifierVariableInstrumentEnum Sensor = Create(7, "E", "Sensor");
        public static ModifierVariableInstrumentEnum TransmitterIndicator = Create(0, "IT", "Transmitter Indicator");
        public static ModifierVariableInstrumentEnum Local = Create(1, "I", "Local");
        public static ModifierVariableInstrumentEnum Suiche = Create(2, "S", "Suiche");
        public static ModifierVariableInstrumentEnum HiSuiche = Create(3, "SH", "High Switch");
        public static ModifierVariableInstrumentEnum LoSuiche = Create(4, "LH", "Low Switch");
        public static ModifierVariableInstrumentEnum ForPump = Create(5, "P", "Pump");
        public static ModifierVariableInstrumentEnum ForMixers = Create(6, "P", "Mixer");

        public static List<ModifierVariableInstrumentEnum> List = new List<ModifierVariableInstrumentEnum>()
        {
            None,Sensor,TransmitterIndicator,Suiche, LoSuiche,HiSuiche,Local, ForPump, ForMixers

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static string GetLetter(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Letter : string.Empty;
        public static ModifierVariableInstrumentEnum GetTypeByName(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static ModifierVariableInstrumentEnum GetTypeByLetter(string letter) => List.Exists(x => x.Letter.Equals(letter)) ? List.FirstOrDefault(x => x.Letter.Equals(letter))! : None;
        public static ModifierVariableInstrumentEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}