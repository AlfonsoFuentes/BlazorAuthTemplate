using Server.Domain.CommonEntities.BudgetItems;
using Shared.Enums.CurrencyEnums;

namespace Server.Domain.CommonEntities.PurchaseOrders
{
    public class PurchaseOrderItem : Entity
    {
        

        public Guid PurchaseOrderId { get; private set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public Guid? BudgetItemId { get; set; }
        public BudgetItem? BudgetItem { get; set; } = null!;

    
        public bool IsTaxNoProductive { get; set; } = false;
        public bool IsTaxAlteration { get; set; } = false;
        [NotMapped]
        public string NomenclatoreName => BudgetItem == null ? string.Empty : BudgetItem.NomenclatoreName;
        [NotMapped]
        public string ItemName => BudgetItem == null ? string.Empty : BudgetItem.Name;
        [NotMapped]
        public double USDCOP => PurchaseOrder == null ? 0 : PurchaseOrder.USDCOP;
        [NotMapped]
        public double USDEUR => PurchaseOrder == null ? 0 : PurchaseOrder.USDEUR;
        [NotMapped]
        public PurchaseOrderStatusEnum PurchaseOrderStatus => PurchaseOrder == null ? PurchaseOrderStatusEnum.None :
            PurchaseOrder.PurchaseOrderStatusEnum;
        public ICollection<PurchaseOrderItemReceived> PurchaseOrderReceiveds { get; set; } = new List<PurchaseOrderItemReceived>();
        public static PurchaseOrderItem Create(Guid purchasorderid, Guid mwobudgetitemid)
        {
            PurchaseOrderItem item = new PurchaseOrderItem();
            item.Id = Guid.NewGuid();
            item.BudgetItemId = mwobudgetitemid;
            item.PurchaseOrderId = purchasorderid;

            return item;
        }
        public PurchaseOrderItemReceived AddPurchaseOrderReceived()
        {
            var row = PurchaseOrderItemReceived.Create(Id);


            return row;
        }
        [NotMapped]
        public CurrencyEnum PurchaseOrderCurrency => PurchaseOrder == null ? CurrencyEnum.None : PurchaseOrder.PurchaseOrderCurrencyEnum;
        [NotMapped]
        public CurrencyEnum QuoteCurrency => PurchaseOrder == null ? CurrencyEnum.None : PurchaseOrder.QuoteCurrencyEnum;     
        public double UnitaryValueQuoteCurrency { get; set; }
        public double Quantity { get; set; }
        [NotMapped]
        public double TotalItemQuoteCurrency => UnitaryValueQuoteCurrency * Quantity;

        [NotMapped]
        public double UnitaryValueUSD => QuoteCurrency.Id == CurrencyEnum.USD.Id ? UnitaryValueQuoteCurrency :
            QuoteCurrency.Id == CurrencyEnum.COP.Id ? USDCOP == 0 ? 0 : UnitaryValueQuoteCurrency / USDCOP :
            QuoteCurrency.Id == CurrencyEnum.EUR.Id ? USDEUR == 0 ? 0 : UnitaryValueQuoteCurrency / USDEUR :
            0;

        [NotMapped]
        public double TotalItemValueUSD => UnitaryValueUSD * Quantity;

        [NotMapped]
        public double UnitaryValuePurchaseOrderCurrency => PurchaseOrderCurrency.Id == CurrencyEnum.USD.Id ? UnitaryValueUSD :
            PurchaseOrderCurrency.Id == CurrencyEnum.COP.Id ? USDCOP == 0 ? 0 : UnitaryValueUSD * USDCOP :
            PurchaseOrderCurrency.Id == CurrencyEnum.EUR.Id ? USDEUR == 0 ? 0 : UnitaryValueUSD * USDEUR :
            0;
        
        [NotMapped]
        public double TotalItemPurchaseOrderCurrency => UnitaryValuePurchaseOrderCurrency * Quantity;        
        [NotMapped]
        public double ActualItemPurchaseOrderCurrency => PurchaseOrderReceiveds == null || PurchaseOrderReceiveds.Count == 0 ? 0 :
            PurchaseOrderReceiveds.Sum(x => x.ValueReceivedCurrency);
        [NotMapped]
        public double CommitmentItemPurchaseOrderCurrency => 
            PurchaseOrderStatus.Id == PurchaseOrderStatusEnum.Created.Id || PurchaseOrderStatus.Id == PurchaseOrderStatusEnum.Closed.Id ? 0 :
            TotalItemPurchaseOrderCurrency - ActualItemPurchaseOrderCurrency;
        [NotMapped]
        public double ActualItemUSD => PurchaseOrderReceiveds == null || PurchaseOrderReceiveds.Count == 0 ? 0 :PurchaseOrderReceiveds.Sum(x => x.ReceivedUSD);
        
        [NotMapped]
        public double PotentialItemUSD =>
            PurchaseOrderStatus.Id == PurchaseOrderStatusEnum.Created.Id ? TotalItemValueUSD : 0;       
        [NotMapped]
        public double CommitmentItemUSD =>
            PurchaseOrderStatus.Id == PurchaseOrderStatusEnum.Created.Id|| PurchaseOrderStatus.Id == PurchaseOrderStatusEnum.Closed.Id ? 0 : TotalItemValueUSD - ActualItemUSD;

       

       

    }
}
