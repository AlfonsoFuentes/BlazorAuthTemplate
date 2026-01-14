//namespace Shared.Dtos.Starts.Investments
//{
    
//    public class InvestmentDto
//    {
//        // ✅ Mantienes todas las listas fuertemente tipadas (sin cambios aquí)
//        public List<AlterationDto> Alterations { get; set; } = new();
//        public List<FoundationDto> Foundations { get; set; } = new();
//        public List<StructuralDto> Structurals { get; set; } = new();
//        public List<EquipmentDto> Equipments { get; set; } = new();
//        public List<ValveDto> Valves { get; set; } = new();
//        public List<ElectricalDto> Electricals { get; set; } = new();
//        public List<PipeDto> Pipes { get; set; } = new();
//        public List<InstrumentDto> Instruments { get; set; } = new();
//        public List<PaintingDto> Paintings { get; set; } = new();
//        public List<EHSDto> EHSs { get; set; } = new();
//        public List<TaxDto> Taxes { get; set; } = new();
//        public List<TestingDto> Testings { get; set; } = new();
//        public List<EngineeringDesignDto> EngineeringDesigns { get; set; } = new();
//        public List<EngineeringSalarysDto> EngineeringSalarys { get; set; } = new(); // ← Nota: corregí typo aquí (Salarys → Salary)
//        public List<ContingencyDto> Contingencies { get; set; } = new();

//        // ✅ Propiedad calculada —pero eficiente y limpia
       
//    }



//    public class GetInvestmentById
//    {
//        public Guid Id { get; set; }
//    }
//    public class BudgetItemSimpleDto : GeneralDto
//    {
//        public Guid Id { get; set; }
//        public string Name { get; set; }     =string.Empty;
//        public double BudgetUSD { get; set; }
//        public string Nomenclatore { get; set; } = string.Empty;
//        public string CategoryName { get; set; } = string.Empty;// "Structural", "Testing", etc.
//    }

//    public class GetBudgetItemsByKnownRiskId
//    {
//        public Guid KnownRiskId { get; set; }
//    }
//    public interface ILinkableToKnownRisk
//    {
//        public Guid? LinkToKnownRiskId { get; set; }
//    }
//    public interface ILinkableToQuality
//    {
//        public Guid? LinkToQualityId { get; set; }
//    }
//}

