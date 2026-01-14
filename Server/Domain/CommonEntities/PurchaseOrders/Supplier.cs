using Shared.Enums.CurrencyEnums;

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
       
        [ForeignKey("SupplierId")]
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
    internal class SupplierConfig : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }
}
