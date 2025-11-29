namespace Shared.Enums.Instruments
{
    public class VariableInstrumentEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Name}";
        }

        public string Letter { get; set; } = string.Empty;
        public static VariableInstrumentEnum Create(int id, string letter, string name) => new VariableInstrumentEnum() { Id = id, Letter = letter, Name = name };
        public static VariableInstrumentEnum None = Create(-1, string.Empty, "None");
        public static VariableInstrumentEnum MassFlowMeter = Create(0, "F", "Mass Flow Meter");
        public static VariableInstrumentEnum VolumeFlowMeter = Create(1, "F", "Volumetric Flow Meter");
        public static VariableInstrumentEnum Temperature = Create(2, "T", "Temperature");
        public static VariableInstrumentEnum Pressure = Create(3, "P", "Pressure");
        public static VariableInstrumentEnum Level = Create(4, "L", "Level");
        public static VariableInstrumentEnum Analitical = Create(5, "A", "Analitical");
        public static VariableInstrumentEnum Weigth = Create(6, "W", "Weight");
        public static VariableInstrumentEnum Frequency_Inverter = Create(7, "VS", "Frequency Inverter");
        public static List<VariableInstrumentEnum> List = new List<VariableInstrumentEnum>()
        {
            None,MassFlowMeter, VolumeFlowMeter, Temperature, Pressure, Level,Weigth, Analitical,Frequency_Inverter

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static string GetLetter(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Letter : string.Empty;
        public static VariableInstrumentEnum GetTypeByName(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static VariableInstrumentEnum GetTypeByLetter(string letter) => List.Exists(x => x.Letter.Equals(letter)) ? List.FirstOrDefault(x => x.Letter.Equals(letter))! : None;
        public static VariableInstrumentEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}