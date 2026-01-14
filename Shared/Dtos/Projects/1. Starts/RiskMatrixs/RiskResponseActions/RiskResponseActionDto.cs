using Shared.Dtos.Plannings.RiskMatrixs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Projects._1._Starts.RiskMatrixs.RiskResponseActions
{
    public class RiskResponseActionDto
    {
        public Guid Id { get; set; }
        public Guid RiskMatrixId { get; set; }

        // Las propiedades editables del plan
        public string Description { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }

        // Importante: Para saber si es Mitigación o Contingencia
        public RiskActionType ActionType { get; set; }

        public int Order {  get; set; }
    }
    public class CreateRiskResponseAction : RiskResponseActionDto
    {
    }

    // --- EDIT: Hereda propiedades ---
    public class EditRiskResponseAction : RiskResponseActionDto
    {
    }

    // --- DELETE: Solo necesita el ID ---
    public class DeleteRiskResponseAction
    {
        public Guid Id { get; set; }
    }

    // --- GET BY ID ---
    public class GetRiskResponseActionById
    {
        public Guid Id { get; set; }
    }

    // --- GET ALL BY PARENT ID ---
    public class GetAllRiskResponseActionsByRiskId
    {
        public Guid RiskMatrixId { get; set; }
    }

    // --- CHANGE ORDER ---
    public class ChangeRiskResponseActionOrder
    {
        public Guid Id { get; set; }
        public int NewOrder { get; set; }
        public Guid RiskMatrixId { get; set; }
    }

}
