using Shared.Dtos.General;
using Shared.Enums.CurrencyEnums;

namespace Shared.Dtos.Suppliers
{
    public class SupplierDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VendorCode { get; set; } = string.Empty;
        public string TaxCodeLD { get; set; } ="751545";
        public string NickName { get; set; } = string.Empty;
        public string TaxCodeLP { get; set; } = "721031";

        public string? PhoneNumber { get; set; } = string.Empty;
        public string? ContactName { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? ContactEmail { get; set; } = string.Empty;
        public CurrencyEnum SupplierCurrency { get; set; } = CurrencyEnum.COP;
    }
    public class CreateSupplier     : SupplierDto
    {
    }
    public class EditSupplier : SupplierDto
    {

    }
    public class GetAllSuppliers
    {

    }
    public class GetSupplierById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateSupplierName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
    }
    public class ValidateSupplierNickName
    {
        public Guid Id { set; get; }
        public string NickName{ set; get; } = string.Empty;
    }
    public class ValidateSupplierVendorCode
    {
        public Guid Id { set; get; }
        public string VendorCode { set; get; } = string.Empty;
    }
    public class DeleteSupplier
    {
        public Guid Id { set; get; }
    }
    public class DeleteGroupSupplier
    {
        public List<Guid> GroupIds { get; set; } = new();
    }
}
