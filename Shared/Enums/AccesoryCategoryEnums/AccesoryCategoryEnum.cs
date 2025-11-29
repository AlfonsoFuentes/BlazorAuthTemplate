namespace Shared.Enums.AccesoryCategoryEnums
{
    public class AccesoryCategoryEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static AccesoryCategoryEnum Create(int id, string name) => new AccesoryCategoryEnum() { Id = id, Name = name };

        public static AccesoryCategoryEnum None = Create(-1, "NONE");
        public static AccesoryCategoryEnum PipeSegment = Create(1, "Pipe");
        public static AccesoryCategoryEnum Ferrule = Create(2, "Ferrule");
        public static AccesoryCategoryEnum Elbow90 = Create(3, "Elbow 90°");
        public static AccesoryCategoryEnum Elbow45 = Create(4, "Elbow 45°");
        public static AccesoryCategoryEnum EqualTeeShortOutlet = Create(5, "Equal Tee Short Outlet");
        public static AccesoryCategoryEnum EqualTeeMiddleShortOutlet = Create(6, "Equal Tee Middle Short Outlet");
        public static AccesoryCategoryEnum EqualTee = Create(7, "Equal Tee");

        public static AccesoryCategoryEnum Reducing_Tee = Create(8, "Reducing Tee");
        public static AccesoryCategoryEnum Reducing_Tee_ShortOutlet = Create(9, "Reducing Tee Short Outlet");
        public static AccesoryCategoryEnum Instrument_Tee = Create(10, "Instrument Tee");
        public static AccesoryCategoryEnum Cross = Create(11, "Cross");
        public static AccesoryCategoryEnum ReducingCross = Create(12, "Reducing Cross");
        public static AccesoryCategoryEnum ConcentricReducer = Create(13, "Concentric Reducer");
        public static AccesoryCategoryEnum EccentricReducer = Create(14, "Eccentric Reducer");
        public static AccesoryCategoryEnum HighClamp = Create(15, "High Pressure Tri-Clamp");
        public static AccesoryCategoryEnum EndCap = Create(16, "End Cap");
        public static AccesoryCategoryEnum Gasket = Create(17, "Gasket");
        public static AccesoryCategoryEnum OrbitalWeld = Create(18, "Orbital Weld");

        public static List<AccesoryCategoryEnum> List = new List<AccesoryCategoryEnum>()
            {
                None,PipeSegment,Ferrule,Elbow90,Elbow45,EqualTeeShortOutlet,EqualTeeMiddleShortOutlet,EqualTee,Reducing_Tee,Reducing_Tee_ShortOutlet,Instrument_Tee,Cross,
                ReducingCross,ConcentricReducer, EccentricReducer, HighClamp, EndCap, Gasket, OrbitalWeld,

            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static AccesoryCategoryEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static AccesoryCategoryEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
    }
}
