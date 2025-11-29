using Shared.Enums.ProjectNeedTypes;

namespace Shared.Enums.Focuses
{
    public class FocusEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }
        public static FocusEnum None = Create(-1, "None");
        public static FocusEnum HomeCare = Create(0, "Home Care");
        public static FocusEnum PersonalCareSoap = Create(1, "Personal Care - Soap");
        public static FocusEnum PersonalCareDEOS = Create(2, "Personal Care - DEOS");
        public static FocusEnum OralCare = Create(3, "Oral Care");
        public static FocusEnum HandDish = Create(4, "Hand Dish");
        public static FocusEnum Engineering = Create(5, "Engineering");

        public static FocusEnum Other = Create(6, "Other");



        public static List<FocusEnum> List = new List<FocusEnum>()
            {
            None, HomeCare, PersonalCareSoap, PersonalCareDEOS,OralCare, HandDish, Engineering, Other
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static FocusEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
        public static FocusEnum Create(int id, string name) => new FocusEnum() { Id = id, Name = name };
        public static FocusEnum GetType(string name) => List.Exists(x => x.Name == name) ? List.FirstOrDefault(x => x.Name == name)!
            : None;
    }
}
