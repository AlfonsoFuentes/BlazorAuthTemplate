using Shared.Dtos.BudgetItems;
using Shared.Enums.BudgetCategorys;
using Shared.Interfaces;

namespace Shared.Dtos.Projects._1._Starts.QualityBudgetItems
{
    public class QualityBudgetItemDto : IModelDto
    {
        public Guid Id { get; set; } // ID de la relación Many-to-Many
        public Guid QualityId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid BudgetItemId { get; set; } // ID del item en la tabla Budget

        // Propiedades del item (se guardan en la tabla BudgetItem)
        public string QualityName { get; set; } = string.Empty;
        public string BudgetName { get; set; } = string.Empty;
        public BudgetCategory Category { get; set; }
        public decimal UnitPriceUSD { get; set; }
        public double Quantity { get; set; }
        public int Order { get; set; }
        public string Nomenclatore { get; set; } = string.Empty;
        public decimal BudgetUSD => UnitPriceUSD * (decimal)Quantity;
    }

    public class CreateQualityBudgetItem : QualityBudgetItemDto { }
    public class EditQualityBudgetItem : QualityBudgetItemDto { }
    public class GetAllQualityBudgetItem
    {
        public Guid QualityId { get; set; }
    }
    public class GetByIdQualityBudgetItem
    {
        public Guid QualityId { get; set; }
        public Guid BudgetItemId { get; set; }
    }
    public class DeleteQualityBudgetItem
    {
        public Guid QualityId { get; set; }
        public Guid BudgetItemId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
