

namespace Server.Domain.CommonEntities.BudgetItems
{
    public abstract class BudgetItem : Entity
    {
        

        public virtual string Letter { get; set; } = string.Empty;
        protected virtual double _BudgetUSD => _SettedBudgetUSD;
        protected virtual double _SettedBudgetUSD { get; set; }
        public double BudgetUSD
        {
            get => _BudgetUSD;
            set => _SettedBudgetUSD = value;
        }
        public bool IsAlteration { get; set; } = false;
        public bool IsTaxes { get; set; } = false;
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        [NotMapped]
        public virtual int OrderList => 0;

        [ForeignKey("SelectedId")]
        public List<TaxesItem> TaxesSelecteds { get; set; } = new();
        [NotMapped]
        public string Nomenclatore => $"{Letter}{Order}";
        public string Name { get; set; } = string.Empty;



        [ForeignKey("BudgetItemId")]
        public List<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new();

        [NotMapped]
        public string NomenclatoreName => $"{Nomenclatore}-{Name}";
        [NotMapped]
        public double ActualUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 : PurchaseOrderItems.Sum(x => x.ActualItemUSD);
        [NotMapped]
        public double CommitmentUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 : PurchaseOrderItems.Sum(x => x.CommitmentItemUSD);
        [NotMapped]
        public double PotentialUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 : PurchaseOrderItems.Sum(x => x.PotentialItemUSD);
        [NotMapped]
        public double AssignedUSD => ActualUSD + CommitmentUSD + PotentialUSD;
        [NotMapped]
        public double ToCommitUSD => BudgetUSD - AssignedUSD;


        [NotMapped]
        public List<PurchaseOrder> PurchaseOrders => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? new() : PurchaseOrderItems.Select(x => x.PurchaseOrder).ToList();

        [NotMapped]
        public List<PurchaseOrder> PurchaseOrderCloseds => PurchaseOrderItems.Count == 0 ? new() : PurchaseOrders.Where(x => x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Closed.Id).ToList();
        [NotMapped]
        public List<PurchaseOrder> PurchaseOrderOpens => PurchaseOrderItems.Count == 0 ? new() : PurchaseOrders.Where(x =>

        x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Approved.Id).ToList();

        [NotMapped]
        public List<PurchaseOrder> PurchaseOrderOpenReceivings => PurchaseOrderItems.Count == 0 ? new() : PurchaseOrders.Where(x =>
        x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Receiving.Id ||
        x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Approved.Id).ToList();
        [NotMapped]
        public List<PurchaseOrder> PurchaseOrderReceivings => PurchaseOrderItems.Count == 0 ? new() : PurchaseOrders.Where(x =>

        x.PurchaseOrderStatus == PurchaseOrderStatusEnum.Receiving.Id).ToList();

        public ICollection<BudgetItemNewGanttTask> BudgetItemNewGanttTasks { get; set; } = new List<BudgetItemNewGanttTask>();

        public List<NewGanttTask> NewGanttTasks => BudgetItemNewGanttTasks.Select(x => x.NewGanttTask).ToList() ?? new();

    }

}
