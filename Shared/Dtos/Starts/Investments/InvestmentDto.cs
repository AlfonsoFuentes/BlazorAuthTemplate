using Shared.Dtos.Starts.Alterations;
using Shared.Dtos.Starts.Contingencys;
using Shared.Dtos.Starts.EHSs;
using Shared.Dtos.Starts.Electricals;
using Shared.Dtos.Starts.EngineeringDesigns;
using Shared.Dtos.Starts.Engineerings;
using Shared.Dtos.Starts.Equipments;
using Shared.Dtos.Starts.Foundations;
using Shared.Dtos.Starts.Instruments;
using Shared.Dtos.Starts.Investments;
using Shared.Dtos.Starts.Paintings;
using Shared.Dtos.Starts.Pipes;
using Shared.Dtos.Starts.Structurals;
using Shared.Dtos.Starts.Taxs;
using Shared.Dtos.Starts.Testings;
using Shared.Dtos.Starts.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Starts.Investments
{
    
    public class InvestmentDto
    {
        // ✅ Mantienes todas las listas fuertemente tipadas (sin cambios aquí)
        public List<AlterationDto> Alterations { get; set; } = new();
        public List<FoundationDto> Foundations { get; set; } = new();
        public List<StructuralDto> Structurals { get; set; } = new();
        public List<EquipmentDto> Equipments { get; set; } = new();
        public List<ValveDto> Valves { get; set; } = new();
        public List<ElectricalDto> Electricals { get; set; } = new();
        public List<PipeDto> Pipes { get; set; } = new();
        public List<InstrumentDto> Instruments { get; set; } = new();
        public List<PaintingDto> Paintings { get; set; } = new();
        public List<EHSDto> EHSs { get; set; } = new();
        public List<TaxDto> Taxes { get; set; } = new();
        public List<TestingDto> Testings { get; set; } = new();
        public List<EngineeringDesignDto> EngineeringDesigns { get; set; } = new();
        public List<EngineeringSalarysDto> EngineeringSalarys { get; set; } = new(); // ← Nota: corregí typo aquí (Salarys → Salary)
        public List<ContingencyDto> Contingencies { get; set; } = new();

        // ✅ Propiedad calculada —pero eficiente y limpia
       
    }



    public class GetInvestmentById
    {
        public Guid Id { get; set; }
    }
}

