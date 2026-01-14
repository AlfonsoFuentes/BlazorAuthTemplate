using Shared.Dtos.BudgetItems;
using Shared.Enums.BudgetCategorys;
using Shared.Interfaces;

namespace Shared.Dtos.Projects._2._Plannings.BudgetItemGanttTasks
{
    public class BudgetItemGanttTaskDto : IModelDto
    {
        public Guid Id { get; set; }
        public Guid GanttTaskId { get; set; }
        public Guid BudgetItemId => BudgetItem?.Id ?? Guid.Empty;
        public Guid ProjectId { get; set; }
        public decimal AmountAssigned { get; set; }

        public BudgetItemDto BudgetItem { get; set; } = null!;
        public string BudgetName => BudgetItem?.Name ?? string.Empty;
        public string Nomenclatore => BudgetItem?.Nomenclatore ?? string.Empty;
        public Shared.Enums.BudgetCategorys.BudgetCategory Category => BudgetItem?.Category ?? Shared.Enums.BudgetCategorys.BudgetCategory.Alteration;
        public decimal TotalBudgetItemUSD => BudgetItem?.BudgetUSD ?? 0;

        // ✅ SALDO DISPONIBLE REAL (Lo que el servidor dice que sobra)
        public decimal AvailableBalance { get; set; }

        // ✅ CUPÓ MÁXIMO PARA ESTA TAREA (Saldo libre + lo que ya tengo)
        // Este es el valor que debe usar el VALIDADOR
        public decimal MaxAllowedAmount => AvailableBalance + _originalAmount;

        // ✅ LO QUE REALMENTE HAN GASTADO OTROS
        // Total - Saldo Libre - Lo que yo tengo
        public decimal SpentByOthers => TotalBudgetItemUSD - AvailableBalance - _originalAmount;

        // ✅ SALDO QUE QUEDARÁ TRAS GUARDAR
        public decimal NewAvailableBalance => MaxAllowedAmount - AmountAssigned;

        public double UsagePercentage => TotalBudgetItemUSD > 0
            ? (double)((AmountAssigned / TotalBudgetItemUSD) * 100)
            : 0;

        // Propiedad privada para guardar el valor inicial al entrar en edición
        private decimal _originalAmount;
        public void SetOriginalAmount(decimal val) => _originalAmount = val;

        public int Order { get; set; }
    }



    public class CreateBudgetItemGanttTask : BudgetItemGanttTaskDto { }
    public class EditBudgetItemGanttTask : BudgetItemGanttTaskDto { }
    public record GetAllBudgetItemGanttTask(Guid GanttTaskId, Guid ProjectId);

    // 2. Para obtener los BudgetItems disponibles del proyecto (con su saldo)
    public record GetAvailableBudgetsForGantt(Guid ProjectId, Guid GanttTaskId);

    // 3. Para borrar una asignación específica
    public record DeleteBudgetItemGanttTask(Guid Id, Guid ProjectId, Guid GanttTaskId);
    public record GetBudgetItemGanttTask(Guid GanttTaskId, Guid BudgetItemId);
    
}
