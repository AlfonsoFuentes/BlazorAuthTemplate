using Shared.Dtos.BudgetItems;
using Shared.Dtos.Projects._1._Starts.RiskMatrixs.RiskResponseActions;
using Shared.Interfaces;

namespace Shared.Dtos.Plannings.RiskMatrixs
{
    public class RiskMatrixDto   :IModelDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public int Order { get; set; }

        // --- Identificación PMP ---
        public string Title { get; set; } = string.Empty; // Nombre corto (ej: "Retraso Aduana")
        public string Cause { get; set; } = string.Empty; // Causa raíz
        public string RiskEvent { get; set; } = string.Empty; // La descripción del evento
        public string Effect { get; set; } = string.Empty; // El impacto en el proyecto

        // Helper para mostrar la frase completa PMP
        public string FullStatement => $"Debido a {Cause}, puede ocurrir {RiskEvent}, causando {Effect}.";

        // --- Análisis Cuantitativo (Heatmap) ---
        public RiskProbability Probability { get; set; } // 1 a 5
        public RiskImpact Impact { get; set; }      // 1 a 5

        // Calculado: Score (Severidad)
        public int RiskScore =>(int) Probability *(int) Impact;

        // Helper Visual: Color para el Badge o la Celda
        public string SeverityColor
        {
            get
            {
                if (RiskScore >= 15) return "Color.Error";   // Rojo (Alto)
                if (RiskScore >= 6) return "Color.Warning";  // Amarillo (Medio)
                return "Color.Success";                      // Verde (Bajo)
            }
        }

        // --- Planificación de Respuesta ---
        public RiskStrategyType StrategyType { get; set; }
        public string ResponsePlanDescription { get; set; } = string.Empty; // Detalle de la acción
        public string Trigger { get; set; } = string.Empty; // Disparador (Cuándo actuar)
        public string Responsible { get; set; } = string.Empty; // Dueño del riesgo
        public RiskStatus Status { get; set; }

        // --- Inversiones Vinculadas (Polimorfismo) ---
        // Aquí mostraremos los chips (Testing, Equipment, etc.)
        public List<BudgetItemDto> LinkedInvestments { get; set; } = new();
        public List<RiskMatrixCommentDto> RiskMatrixComments { get; set; } = new();
        public List<RiskResponseActionDto> RiskResponseActions { get; set; } = new();
    }

    // --- DTOs para Creación y Edición ---

    public class CreateRiskMatrix   : RiskMatrixDto
    {
       
    }

    public class EditRiskMatrix : RiskMatrixDto
    {
      
    }
     public class GetBudgetItemsByRiskMatrixId
    {
        public Guid RiskMatrixId {  get; set; }
    }
    public class GetAllRiskMatrixs
    {
        public Guid ProjectId { get; set; }
    }

    public class GetRiskMatrixById
    {
        public Guid Id { get; set; }
    }

    public class DeleteRiskMatrix
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // Para validación y reordenamiento
    public class ValidateRiskMatrixTitle
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
    }

    public class ChangeOrderRiskMatrix
    {
        public Guid Id { get; set; }
        public int NewOrder { get; set; }
        public Guid ProjectId { get; set; }
    }
    public class RiskMatrixCommentDto
    {
        public Guid Id { get; set; }
        public Guid RiskMatrixId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CommentDate { get; set; }
        public string CommentedBy { get; set; } = string.Empty; // Nombre del usuario
    }

    // DTO de Creación
    public class CreateRiskMatrixComment : RiskMatrixCommentDto
    {

    }
    public class EditRiskMatrixComment : RiskMatrixCommentDto
    {

    }

    // DTO de Eliminación (opcional, si permites borrar historial)
    public class DeleteRiskMatrixComment
    {
        public Guid Id { get; set; }
        public Guid RiskMatrixId { get; set; } // Para validar o invalidar caché
    }
    public interface ILinkableToRiskMatrix
    {
        // Esta propiedad permitirá que cualquier DTO (CreateTesting, CreateValve, etc.)
        // transporte el ID de la Matriz de Riesgo para hacer el vínculo automático.
        public Guid? LinkToRiskMatrixId { get; set; }
    }
}
