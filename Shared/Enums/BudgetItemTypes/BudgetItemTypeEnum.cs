namespace Shared.Enums.BudgetItemTypes
{
    public class BudgetItemTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Letter}-{Name}";
        }

        public string Letter { get; set; } = string.Empty;
        public static BudgetItemTypeEnum Create(int id, string letter, string name) => new BudgetItemTypeEnum() { Id = id, Letter = letter, Name = name };

        public static BudgetItemTypeEnum None = Create(-1, "", "None");
        public static BudgetItemTypeEnum Alteration = Create(0, "A", "Alterations");
        public static BudgetItemTypeEnum Foundation = Create(1, "B", "Foundations");
        public static BudgetItemTypeEnum Structural = Create(2, "C", "Structural");
        public static BudgetItemTypeEnum Equipment = Create(3, "D", "Equipments");
        public static BudgetItemTypeEnum Electrical = Create(4, "E", "Electrical");
        public static BudgetItemTypeEnum Piping = Create(5, "F", "Piping");
        public static BudgetItemTypeEnum Instruments = Create(6, "G", "Instruments");
        public static BudgetItemTypeEnum Insulations = Create(7, "H", "Insulation");
        public static BudgetItemTypeEnum Painting = Create(8, "I", "Painting");
        public static BudgetItemTypeEnum EHS = Create(9, "K", "EHS");

        public static BudgetItemTypeEnum Taxes = Create(10, "L", "Taxes");
        public static BudgetItemTypeEnum Testing = Create(11, "N", "Testing");
        public static BudgetItemTypeEnum Engineering = Create(12, "O", "Engineering");
        public static BudgetItemTypeEnum Contingency = Create(13, "P", "Contingency");
        public static BudgetItemTypeEnum Valves = Create(14, "D", "Valves");
        public static List<BudgetItemTypeEnum> List = new List<BudgetItemTypeEnum>()
        {
            None,Alteration,Foundation,Structural, Equipment,Valves,Electrical,Piping,Instruments, Insulations,Painting, EHS, Taxes, Testing, Engineering, Contingency
        };
        public static List<BudgetItemTypeEnum> ListForCreateItem = new List<BudgetItemTypeEnum>()
        {
            None,Alteration,Foundation,Structural, Equipment,Valves,Electrical,Piping,Instruments, Insulations,Painting, EHS, Taxes, Testing, Engineering
        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static string GetLetter(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Letter : string.Empty;
        public static BudgetItemTypeEnum GetTypeByName(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static BudgetItemTypeEnum GetTypeByLetter(string letter) => List.Exists(x => x.Letter.Equals(letter)) ? List.FirstOrDefault(x => x.Letter.Equals(letter))! : None;
        public static BudgetItemTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Letter;
            yield return base.GetEqualityComponents();

        }
    }
}
