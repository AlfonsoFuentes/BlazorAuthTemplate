using Shared.Enums.CostCenter;
using Shared.Enums.CurrencyEnums;

namespace Server.Domain.CommonEntities.PurchaseOrders
{
    public class PurchaseOrder : Entity
    {
        
        public Project Project { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; } = null!;
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public string QuoteNo { get; set; } = string.Empty;
        public string PurchaseRequisition { get; set; } = string.Empty;
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public string SPL { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public double USDCOP { get; set; }
        public double USDEUR { get; set; }
        public DateTime CurrencyDate { get; set; }
        public string ProjectAccount { get; set; } = string.Empty;
        public string PurchaseorderName { get; set; } = string.Empty;
        public bool IsAlteration { get; set; } = false;
        public bool IsCapitalizedSalary { get; set; } = false;
        public bool IsTaxEditable { get; set; } = false;
        public bool IsNotEditable { get; set; } = false;
        public bool IsProductiveAsset { get; set; } = false;
        public int QuoteCurrency { get; set; }
        public int PurchaseOrderCurrency { get; set; }
        public int PurchaseOrderStatus { get; set; }
        public int CostCenter { get; set; }
        public Guid MainBudgetItemId { get; set; } = Guid.Empty;
        [NotMapped]
        public CostCenterEnum CostCenterEnum => CostCenterEnum.GetType(CostCenter);
        [NotMapped]
        public PurchaseOrderStatusEnum PurchaseOrderStatusEnum => PurchaseOrderStatusEnum.GetType(PurchaseOrderStatus);
        [NotMapped]
        public CurrencyEnum PurchaseOrderCurrencyEnum => CurrencyEnum.GetType(PurchaseOrderCurrency);
        [NotMapped]
        public CurrencyEnum QuoteCurrencyEnum => CurrencyEnum.GetType(QuoteCurrency);
        public static PurchaseOrder Create(Guid projectid)
        {
            return new()
            {
                ProjectId = projectid,
                Id = Guid.NewGuid(),

            };
            
        }
        [NotMapped]
        public string SupplierName => Supplier == null ? string.Empty : Supplier.Name;
        [NotMapped]
        public string SupplierNickName => Supplier == null ? string.Empty : Supplier.NickName;
        [NotMapped]
        public double TotalPurchaseOrderCurrency => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 :
            PurchaseOrderItems.Sum(x => x.TotalItemPurchaseOrderCurrency);
        [NotMapped]
        public double TotalQuoteCurrency => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 :
            PurchaseOrderItems.Sum(x => x.TotalItemQuoteCurrency);
        [NotMapped]
        public double TotalUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 :
           PurchaseOrderItems.Sum(x => x.TotalItemValueUSD);
        [NotMapped]
        public double ActualCurrency => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 :
            PurchaseOrderItems.Sum(x => x.ActualItemPurchaseOrderCurrency);
        [NotMapped]
        public double ActualUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 :
            PurchaseOrderItems.Sum(x => x.ActualItemUSD);

        [NotMapped]
        public double PotentialCommitmentUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 :
            PurchaseOrderItems.Sum(x => x.PotentialItemUSD);

        [NotMapped]
        public double CommitmentUSD => PurchaseOrderItems == null || PurchaseOrderItems.Count == 0 ? 0 :
            PurchaseOrderItems.Sum(x => x.CommitmentItemUSD);
        
        
       
    }
}
