using Shared.Enums.CurrencyEnums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.CommonEntities.PurchaseOrders
{
    public class Supplier : Entity
    {

        public string Name { get; set; } = string.Empty;
        public string VendorCode { get; set; } = string.Empty;
        public string TaxCodeLD { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string TaxCodeLP { get; set; } = string.Empty;
       
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? ContactName { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? ContactEmail { get; set; } = string.Empty;
        public int SupplierCurrency { get; set; } = 0;
        [NotMapped]
        public CurrencyEnum SupplierCurrencyEnum => CurrencyEnum.GetType(SupplierCurrency);
        public static Supplier Create(string Name, string VendorCode, string TaxCodeLD, string TaxCodeLP, int SupplierCurrency)
        {
            return new Supplier()
            {
                Name = Name,
                VendorCode = VendorCode,
                TaxCodeLD = TaxCodeLD,
                TaxCodeLP = TaxCodeLP,
                SupplierCurrency = SupplierCurrency
            };

        }
        public static Supplier Create()
        {
            return new() { Id = Guid.NewGuid() };
        }
        [ForeignKey("SupplierId")]
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
