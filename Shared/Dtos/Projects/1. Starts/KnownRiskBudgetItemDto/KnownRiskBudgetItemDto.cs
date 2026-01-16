using Shared.Enums.BudgetCategorys;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Projects._1._Starts.KnownRiskBudgetItemDto
{
    public class KnownRiskBudgetItemDto : IModelDto
    {
        public Guid Id { get; set; } // ID de la relación Many-to-Many
        public Guid KnownRiskId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid BudgetItemId { get; set; } // ID del item en la tabla Budget

        // Propiedades del item (se guardan en la tabla BudgetItem)
        public string KnownRiskName { get; set; } = string.Empty;
        public string BudgetName { get; set; } = string.Empty;
        public BudgetCategory Category { get; set; }
        public decimal UnitPriceUSD { get; set; }
        public double Quantity { get; set; }
        public int Order { get; set; }
        public string Nomenclatore { get; set; } = string.Empty;
        public decimal BudgetUSD => UnitPriceUSD * (decimal)Quantity;
    }

    public class CreateKnownRiskBudgetItem : KnownRiskBudgetItemDto { }
    public class EditKnownRiskBudgetItem : KnownRiskBudgetItemDto { }
    public class GetAllKnownRiskBudgetItem
    {
        public Guid KnownRiskId { get; set; }
    }
    public class GetByIdKnownRiskBudgetItem
    {
        public Guid KnownRiskId { get; set; }
        public Guid BudgetItemId { get; set; }
    }
    public class DeleteKnownRiskBudgetItem
    {
        public Guid KnownRiskId { get; set; }
        public Guid BudgetItemId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
