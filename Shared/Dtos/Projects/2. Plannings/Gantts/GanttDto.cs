using Shared.Dtos.Projects._2._Plannings.BudgetItemGanttTasks;
using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Dtos.Projects.Plannings.Gantts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Projects.Plannings.Gantts
{

    public class GanttDependencyDto
    {
        public Guid Id { get; set; }
        public GanttDto? Predecessor
        {
            get;
            set
            {
                field = value; // Asigna al backing-field generado por el compilador

                // Sincronización automática: 
                // Si hay objeto, toma el ID. Si es null, pone Empty.
                PredecessorId = value?.Id ?? Guid.Empty;
            }
        }

        // ✅ Mantenemos el setter para que la deserialización (JSON) funcione
        // si la API envía solo el ID sin el objeto anidado.


        // ✅ Mantenemos el set para que la API pueda llenarlo al leer de DB sin necesitar el objeto completo
        public Guid PredecessorId { get;  set; }
        public DependencyType Type { get; set; }
        public string Lag { get; set; } = "0d";
        public int Order { get; set; }


        // ✅ Agregamos estos campos para el control de conflictos
        public bool IsCircularConflict { get; set; }
        public string ConflictMessage { get; set; } = string.Empty;
    }



    public class GanttDto
    {
        public int IdNumber { get; set; } // ✅ 1, 2, 3, ... (para UI)
        public string WbsCode { get; set; } = string.Empty; // ✅ "1", "1.1", "2.1"
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Duration { get; set; } = "1d";
        //public DurationUnit DurationUnit { get; set; } = DurationUnit.Days;
        public bool IsExpanded { get; set; } = true;
        public List<GanttDto> Children { get; set; } = new();
        public List<GanttDependencyDto> Dependencies { get; set; } = new();
        public bool IsMilestone { get; set; }
        //public CalendarType Calendar { get; set; } = CalendarType.BusinessDays;
        public string ResponsibleId { get; set; } = string.Empty;
        //public string Phase { get; set; } = string.Empty;
        //public GanttInputMode InputMode { get; set; } = GanttInputMode.Undefined;
        public GanttField? LastModifiedField { get; set; }
        //public GanttField? PreviousModifiedField { get; set; } // 🔹 nuevo
        public string SummaryDependencies { get; set; } = string.Empty;
        // 🔹 Método para registrar cambios (usar en UI)

        public List<CommunicationDto> Communications { get; set; } = new();
        public decimal Capital { get; set; }
        public decimal Expenses { get; set; }
        public decimal Appropiation => Capital + Expenses;
        public List<BudgetItemGanttTaskDto> BudgetAssignments { get; set; } = new();
    }

    public class CreateGantt : GanttDto { }
    public class EditGantt : GanttDto { }



    public record DeleteGanttTask(Guid Id, Guid ProjectId);

    public record IndentGanttTaskRight(Guid Id, Guid ProjectId, Guid TargetParentId);
    public record IndentGanttTaskLeft(Guid Id, Guid ProjectId, Guid? NewParentId); // null = raíz

    public record MoveGanttTaskUp(Guid Id, Guid ProjectId);
    public record MoveGanttTaskDown(Guid Id, Guid ProjectId);
    public record GetGanttTaskById(Guid Id);
    public record GetAllGanttTasks(Guid ProjectId);
   
    public record ValidateGanttTaskName(Guid ProjectId, Guid Id, string Name);
    public record GetMonthlyExpendByProject(Guid ProjectId);

    public class MonthlyExpenditureResponse
    {
        public List<string> Columns { get; set; } = new();
        public List<MonthlyExpenditureRow> Rows { get; set; } = new();
    }

    public class MonthlyExpenditureRow
    {
        public string BudgetName { get; set; } = string.Empty;
        public string Nomenclatore { get; set; } = string.Empty;
        public decimal OriginalBudget { get; set; } // Presupuesto base del BudgetItem
        public bool IsSummary { get; set; }
        public bool IsVirtual { get; set; }
        public Dictionary<string, decimal> MonthlyValues { get; set; } = new();

        public decimal Total => MonthlyValues.Values.Sum();
        public decimal Unassigned => OriginalBudget - Total;

        public void AddAmount(string month, decimal amount)
        {
            if (!MonthlyValues.ContainsKey(month)) MonthlyValues[month] = 0;
            MonthlyValues[month] += amount;
        }
    }
}
