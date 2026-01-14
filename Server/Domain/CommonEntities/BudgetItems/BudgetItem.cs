
using Shared.ExtensionsMethods;
using Shared.Enums.BudgetCategorys;

namespace Server.Domain.CommonEntities.BudgetItems
{


    public class BudgetItem : Entity
    {
        // --- 1. IDENTIFICACIÓN Y CONFIGURACIÓN ---
     
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; } = default!;

        // Aquí vive la magia: El Enum con Atributos
        public BudgetCategory Category { get; set; }

        public string Name { get; set; } = string.Empty; // Descripción corta
        public string Description { get; set; } = string.Empty; // Descripción larga (opcional)
        public string Unit { get; set; } = "UND";

        // --- 2. VALORES FINANCIEROS (Inputs & Persistencia) ---

        public double Quantity { get; set; } = 1;

        // En Directos: Input manual. En Especiales: Calculado por el motor.
        public decimal UnitPriceUSD { get; set; }

        // VALOR PERSISTIDO EN BD
        // Se calcula como (Qty * UnitPrice) al guardar o por el BudgetCalculator.
        public decimal BudgetUSD { get; set; }


        // --- 3. LÓGICA DE CLASIFICACIÓN (Helpers en Memoria) ---

        [NotMapped]
        public string Letter => Category.GetLetter(); // Usa la extensión/atributo

        [NotMapped]
        public string Nomenclatore => $"{Category.GetLetter()}{Order}"; // Ej: "A1", "F5"

        [NotMapped]
        public string NomenclatoreName => $"{Nomenclatore}-{Name}"; // Ej: "F5-Tubería Principal"

        // Helpers Booleanos para tu lógica de negocio
        [NotMapped]
        public bool IsExpense => Category == BudgetCategory.Alteration; // Reemplaza a IsAlteration manual

        [NotMapped]
        public bool IsCapital => !IsExpense;

        [NotMapped]
        public bool IsSystemCalculated => Category.IsSpecialCalculation(); // Tax, Eng, Cont

        // Esto reemplaza tu "IsTaxes" manual
        [NotMapped]
        public bool IsTaxes => Category == BudgetCategory.Tax;

        [NotMapped]
        public bool IsNoExpenseTaxesEngCont =>
            Category != BudgetCategory.Tax ||
            Category != BudgetCategory.Alteration ||
            Category != BudgetCategory.Engineering ||
            Category != BudgetCategory.Contingency;


        // --- 4. RELACIONES (PURCHASE ORDERS - Mantenidas igual) ---

        [ForeignKey("BudgetItemId")]
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        // Cálculos de Ejecución (Estos dependen de las POs, se quedan calculados)
        [NotMapped]
        public double ActualUSD => PurchaseOrderItems?.Sum(x => x.ActualItemUSD) ?? 0;

        [NotMapped]
        public double CommitmentUSD => PurchaseOrderItems?.Sum(x => x.CommitmentItemUSD) ?? 0;

        [NotMapped]
        public double PotentialUSD => PurchaseOrderItems?.Sum(x => x.PotentialItemUSD) ?? 0;

        [NotMapped]
        public double AssignedUSD => ActualUSD + CommitmentUSD + PotentialUSD;

        [NotMapped]
        public double ToCommitUSD => (double)BudgetUSD - AssignedUSD; // Cuanto me queda por gastar

        // Listas de Ayuda para POs
        [NotMapped]
        public List<PurchaseOrder> PurchaseOrders => PurchaseOrderItems?.Select(x => x.PurchaseOrder).Distinct().ToList() ?? new();

        [NotMapped]
        public List<PurchaseOrder> PurchaseOrderCloseds => PurchaseOrders.Where(x => x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Closed.Id).ToList();

        [NotMapped]
        public List<PurchaseOrder> PurchaseOrderOpens => PurchaseOrders.Where(x => x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Approved.Id).ToList();

        [NotMapped]
        public List<PurchaseOrder> PurchaseOrderReceivings => PurchaseOrders.Where(x => x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Receiving.Id).ToList();




        [ForeignKey("BudgetItemId")]
        public virtual ICollection<BudgetItemGanttTask> BudgetItemGanttTasks { get; set; } = new List<BudgetItemGanttTask>();

        [NotMapped]
        public List<GanttTask> RelatedGanttTasks => BudgetItemGanttTasks?.Select(x => x.GanttTask).ToList() ?? new();

        // RISKS
        [ForeignKey("BudgetItemId")]
        public virtual ICollection<RiskBudgetItem> RiskBudgetItems { get; set; } = new List<RiskBudgetItem>();

        [NotMapped]
        public List<RiskMatrix> RelatedRisks => RiskBudgetItems?.Select(x => x.RiskMatrix).ToList() ?? new();

        // KNOWN RISKS
        [ForeignKey("BudgetItemId")]
        public virtual ICollection<KnownRiskBudgetItem> KnownRiskBudgetItems { get; set; } = new List<KnownRiskBudgetItem>();

        [NotMapped]
        public List<KnownRisk> RelatedKnownRisks => KnownRiskBudgetItems?.Select(x => x.KnownRisk).ToList() ?? new();

        // QUALITY
        [ForeignKey("BudgetItemId")]
        public virtual ICollection<QualityBudgetItem> QualityBudgetItems { get; set; } = new List<QualityBudgetItem>();

        [NotMapped]
        public List<Quality> RelatedQualityItems => QualityBudgetItems?.Select(x => x.Quality).ToList() ?? new();
    }
    internal class BudgetItemItemConfig : IEntityTypeConfiguration<BudgetItem>
    {
        public void Configure(EntityTypeBuilder<BudgetItem> builder)
        {
            builder.HasQueryFilter(x => x.IsDeleted == false);
            builder.HasMany(x => x.PurchaseOrderItems)
                .WithOne(x => x.BudgetItem)
                .HasForeignKey(x => x.BudgetItemId)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
