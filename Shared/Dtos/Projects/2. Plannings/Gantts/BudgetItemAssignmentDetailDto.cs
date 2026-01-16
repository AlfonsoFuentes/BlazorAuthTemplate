using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Projects._2._Plannings.Gantts
{
    public class BudgetItemAssignmentDetailDto
    {
        public string TaskName { get; set; } = string.Empty;
        public DateTime? EndDate { get; set; }
        public decimal AmountAssigned { get; set; }
        public double Progress { get; set; } // Progreso de la tarea
    }
    public record GetBudgetItemAssignmentDetail(Guid BudgetItemId)  ;
}
