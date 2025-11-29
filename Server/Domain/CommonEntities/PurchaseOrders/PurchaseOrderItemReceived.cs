using Shared.Enums.CurrencyEnums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.CommonEntities.PurchaseOrders
{
    public class PurchaseOrderItemReceived : Entity
    {
        
        public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
        public Guid PurchaseOrderItemId { get; set; }

        public static PurchaseOrderItemReceived Create(Guid purchaseorderitemId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                PurchaseOrderItemId = purchaseorderitemId,

            };
        }

        public double ValueReceivedCurrency { get; set; }
        public double USDCOP { get; set; }
        public double USDEUR { get; set; }
        public DateTime? CurrencyDate { get; set; }
        [NotMapped]
        public string NomenclatoreName => PurchaseOrderItem == null ? string.Empty : PurchaseOrderItem.NomenclatoreName;
        [NotMapped]
        public string ItemName => PurchaseOrderItem == null ? string.Empty : PurchaseOrderItem.ItemName;
        [NotMapped]
        public CurrencyEnum PurchaseOrderCurrency => PurchaseOrderItem == null ? CurrencyEnum.None : PurchaseOrderItem.PurchaseOrderCurrency;

        [NotMapped]
        public double ReceivedUSD =>
           PurchaseOrderCurrency.Id == CurrencyEnum.USD.Id ? ValueReceivedCurrency :
           PurchaseOrderCurrency.Id == CurrencyEnum.COP.Id ? USDCOP == 0 ? 0 : ValueReceivedCurrency / USDCOP :
           PurchaseOrderCurrency.Id == CurrencyEnum.EUR.Id ? USDEUR == 0 ? 0 : ValueReceivedCurrency / USDEUR :
             0;
    }
}
