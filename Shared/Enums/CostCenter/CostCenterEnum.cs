namespace Shared.Enums.CostCenter
{
    public class CostCenterEnum:ValueObject
    {
        public override string ToString()
        {
            return Name;
        }
        
        public static CostCenterEnum Create(int id, string name) => new CostCenterEnum() { Id = id, Name = name };

        public static CostCenterEnum None = Create(-1, "None");
        public static CostCenterEnum Saponification = Create(0, "1200");
        public static CostCenterEnum PCPFinishing = Create(1, "1202");
        public static CostCenterEnum MKDEOS = Create(1, "1230");
        public static CostCenterEnum HCMaking = Create(2, "1300");
        public static CostCenterEnum HCSachets = Create(2, "1360");
        public static CostCenterEnum HCBottles = Create(2, "1340");
        public static CostCenterEnum OCMaking = Create(2, "1105");
        public static List<CostCenterEnum> List = new List<CostCenterEnum>()
        {
            None, Saponification, PCPFinishing,MKDEOS, HCMaking,HCSachets,HCBottles, OCMaking
        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static CostCenterEnum GetTypeByName(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;

        public static CostCenterEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;


    }
}
