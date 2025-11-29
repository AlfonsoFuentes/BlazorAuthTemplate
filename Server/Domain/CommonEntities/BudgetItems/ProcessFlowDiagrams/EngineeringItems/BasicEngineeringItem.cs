namespace Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.EngineeringItems
{
    //public abstract class BasicEngineeringItem : Entity
    //{
    //    public double BudgetUSD { get; set; }
    //    public string Name { get; set; } = string.Empty;
        
    //    public string TagNumber { get; set; } = string.Empty;
    //    public string TagLetter { get; set; } = string.Empty;
    //    public string ProvisionalTag { get; set; } = string.Empty;
    //    public string Tag => $"{TagLetter}-{TagNumber}";
    //    public List<Nozzle> Nozzles { get; set; } = new List<Nozzle>();

    //    [ForeignKey("BasicEngineeringItemConnectedId")]
    //    public ICollection<Nozzle> ItemConnecteds { get; set; } = new List<Nozzle>();

    //    public bool IsExisting { get; set; }
    //    [NotMapped]
    //    public virtual int OrderList => 0;
    //    public Project Project { get; set; } = null!;
    //    public virtual Guid ProjectId { get; set; }

    //    //[ForeignKey("SelectedBasicEngineeringItemId")]
    //    //public ICollection<BudgetItemNewGanttTask> BudgetItemNewGanttTasks { get; set; } = new List<BudgetItemNewGanttTask>();
    //    //[ForeignKey("BasicEngineeringItemId")]
    //    //public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    //    //[NotMapped]
    //    //public double ToCommitUSD => BudgetUSD - AssignedUSD;
    //    //[NotMapped]
    //    //public double AssignedUSD => ActualUSD + CommitmentUSD + PotentialUSD;
    //    //[NotMapped]
    //    //public double ActualUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 : PurchaseOrderItems.Sum(x => x.ActualItemUSD);
    //    //[NotMapped]
    //    //public double CommitmentUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 : PurchaseOrderItems.Sum(x => x.CommitmentItemUSD);
    //    //[NotMapped]
    //    //public double PotentialUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 : PurchaseOrderItems.Sum(x => x.PotentialItemUSD);


    //}
}
