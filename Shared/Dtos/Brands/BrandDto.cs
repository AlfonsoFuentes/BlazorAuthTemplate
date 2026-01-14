using Shared.Dtos.General;
using Shared.Enums.CurrencyEnums;
using Shared.Interfaces;

namespace Shared.Dtos.Brands
{
    public class BrandDto : IModelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }

    }
    public class CreateBrand     : BrandDto
    {
    }
    public class EditBrand : BrandDto
    {

    }
    public class GetAllBrands
    {

    }
    public class GetBrandById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateBrandName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
    }
    public class ValidateBrandNickName
    {
        public Guid Id { set; get; }
        public string NickName{ set; get; } = string.Empty;
    }
    public class ValidateBrandVendorCode
    {
        public Guid Id { set; get; }
        public string VendorCode { set; get; } = string.Empty;
    }
    public class DeleteBrand
    {
        public Guid Id { set; get; }
    }
    public class DeleteGroupBrand
    {
        public List<Guid> GroupIds { get; set; } = new();
    }
}
