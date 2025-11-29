using Server.Domain.CommonEntities.BudgetItems;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class NewGanttTask : Entity
    {
        public Deliverable Deliverable { get; set; } = null!;
        public Guid DeliverableId { get; set; }
        
        public string Name { get; set; } = string.Empty;
        [NotMapped]
        public Guid ProjectId => Deliverable == null ? Guid.Empty : Deliverable.ProjectId;
        public int InternalOrder { get; set; }
        public int TaskStatus { get; set; } // Representa el estado de la tarea
        public string ParentWBS { get; set; } = string.Empty;
        [NotMapped]
        public string WBS => $"{ParentWBS}.{InternalOrder}";
        public int MainOrder { get; set; }
      
        public static NewGanttTask Create(Guid DeliverableId)
        {
            return new()
            {
                Id = Guid.NewGuid(),

                DeliverableId = DeliverableId,


            };
        }
        public static NewGanttTask AddSubTask(Guid ParenTaskId, Guid DeliverableId)
        {
            return new()
            {
                Id = Guid.NewGuid(),

                ParentId = ParenTaskId,
                DeliverableId = DeliverableId,

            };
        }


        // Relación padre-hijo
        public Guid? ParentId { get; set; } // Referencia al padre (opcional)
        public NewGanttTask Parent { get; set; } = null!;
        [ForeignKey("ParentId")]
        public List<NewGanttTask> SubTasks { get; set; } = new List<NewGanttTask>(); // Colección de subtareas
        public bool IsMilestone { get; set; } = false;
        public string? DurationUnit { get; set; } = string.Empty;
        public double DurationInDays { get; set; } = 0;
        public double DurationInUnit { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? RealDurationUnit { get; set; } = string.Empty;
        public double RealDurationInDays { get; set; } = 0;
        public double RealDurationInUnit { get; set; } = 0;
        public DateTime? RealStartDate { get; set; }
        public DateTime? RealEndDate { get; set; }

        public ICollection<BudgetItemNewGanttTask> BudgetItemNewGanttTasks { get; set; } = new List<BudgetItemNewGanttTask>();
        public double TotalBudgetAssigned => BudgetItemNewGanttTasks == null || BudgetItemNewGanttTasks.Count == 0 ? 0 : BudgetItemNewGanttTasks.Sum(x => x.GanttTaskBudgetAssigned);

        [ForeignKey("MainTaskId")]
        public ICollection<MainTaskDependency> MainTasks { get; set; } = new List<MainTaskDependency>();
        [ForeignKey("DependencyTaskId")]
        public ICollection<MainTaskDependency> DependencyTasks { get; set; } = new List<MainTaskDependency>();

    }
}
