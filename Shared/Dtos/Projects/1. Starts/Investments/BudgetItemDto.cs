using Shared.Enums;
using Shared.Enums.BudgetCategorys;
using Shared.ExtensionsMethods;
using Shared.Interfaces;

namespace Shared.Dtos.BudgetItems
{
    public class BudgetItemDto : IModelDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public int Order { get; set; }

        public string Name { get; set; } = string.Empty;

        // ✅ Quantity sigue siendo double (permite decimales como 1.5 metros)
        public double Quantity { get; set; } = 1;

        // ✅ Dinero siempre en Decimal
        public decimal UnitPriceUSD { get; set; }

        // ✅ Tu fórmula exacta: Casteamos Quantity para operar con Decimal
        public decimal BudgetUSD => (decimal)Quantity * UnitPriceUSD;

        public BudgetCategory Category { get; set; }
        public string Nomenclatore => $"{Category.GetLetter()}{Order}";
        public bool IsEditable => Category != BudgetCategory.Tax
                            && Category != BudgetCategory.Engineering
                            && Category != BudgetCategory.Contingency;

        public bool IsExpense=>Category == BudgetCategory.Alteration;

        public bool IsCapital=>!IsExpense;
    }

    // --- Comandos ---
    public class CreateBudgetItem : BudgetItemDto { }
    public class EditBudgetItem : BudgetItemDto { }

    public class DeleteBudgetItem
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    public class ChangeOrderBudgetItem
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public int NewOrder { get; set; }
        public BudgetCategory Category { get; set; }
    }

    // --- Queries ---
    public class GetAllBudgetItems
    {
        public Guid ProjectId { get; set; }
    }

    public class GetBudgetItemById
    {
        public Guid Id { get; set; }
    }

    public class ValidateBudgetItemName
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public BudgetCategory Category { get; set; }
    }
}