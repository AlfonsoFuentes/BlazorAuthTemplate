using Shared.Enums.BudgetCategorys;
using Shared.Enums.Hazops;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Projects._1._Starts.Hazops
{
    public class HazopNodeDto : IModelDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DesignIntent { get; set; } = string.Empty;

        // Relación 1:N con los detalles del análisis
        public List<HazopDetailDto> Details { get; set; } = new();
    }

    public class HazopDetailDto : IModelDto
    {
        public Guid Id { get; set; }
        public Guid HazopNodeId { get; set; }
        public Guid ProjectId { get; set; }
        public HazopParameter Parameter { get; set; } = HazopParameter.None;
        public HazopGuideWord GuideWord { get; set; } = HazopGuideWord.None;

        // Propiedad calculada para la UI (como tu FullStatement de Riesgos)
        public string Deviation => $"{GuideWord} {Parameter}";
        public string Causes { get; set; } = string.Empty;
        public string Consequences { get; set; } = string.Empty;
        public string Safeguards { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public int Order { get; set; }
    }
    public class CreateHazopDetailNode : HazopDetailDto { }
    public class EditHazopDetailNode : HazopDetailDto { }

    public class GetHazopDetailById
    {
        public Guid Id { get; set; }


    }
    public class DeleteHazopDetailNode
    {
        public Guid Id { get; set; }
        public Guid HazopNodeId { get; set; }
        public Guid ProjectId { get; set; }
    }
    // --- Comandos para Endpoints ---
    public class CreateHazopNode : HazopNodeDto { }
    public class EditHazopNode : HazopNodeDto { }
    public class GetAllHazopNodes { public Guid ProjectId { get; set; } }
    public class GetHazopNodeById { public Guid Id { get; set; } }
    public class DeleteHazopNode { public Guid Id { get; set; } public Guid ProjectId { get; set; } }
    public class ValidateHazopName
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
  
    }
}
