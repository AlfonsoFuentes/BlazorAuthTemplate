using Shared.Enums.StakeHolderTypes;

namespace Shared.Enums.ProjectNeedTypes
{
    public class ProjectNeedTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }
        public static ProjectNeedTypeEnum None = Create(-1, "None");
        public static ProjectNeedTypeEnum MarketDemand = Create(0, "Market Demand");
        public static ProjectNeedTypeEnum OrganizationNeeds = Create(1, "Organization Needs");
        public static ProjectNeedTypeEnum QualityImprovement = Create(2, "Quality Improvement");
        public static ProjectNeedTypeEnum EHSImprovement = Create(3, "EHS Improvement");
        public static ProjectNeedTypeEnum TechnologyAdvance = Create(4, "Technology Advance");
        public static ProjectNeedTypeEnum LegalRequirement = Create(5, "Legal Requirement");
        public static ProjectNeedTypeEnum Saving = Create(6, "Saving");
        public static ProjectNeedTypeEnum EcologicalImpact = Create(7, "Ecological Impact");
        public static ProjectNeedTypeEnum SocialNeed = Create(8, "Social Need");
        public static ProjectNeedTypeEnum Other = Create(9, "Other");



        public static List<ProjectNeedTypeEnum> List = new List<ProjectNeedTypeEnum>()
            {
            None,MarketDemand,OrganizationNeeds,QualityImprovement,EHSImprovement,TechnologyAdvance,LegalRequirement,Saving,EcologicalImpact,SocialNeed,Other
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static ProjectNeedTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
        public static ProjectNeedTypeEnum Create(int id, string name) => new ProjectNeedTypeEnum() { Id = id, Name = name };
        public static ProjectNeedTypeEnum GetType(string name) => List.Exists(x => x.Name == name) ? List.FirstOrDefault(x => x.Name == name)!
            : None;
    }
}
