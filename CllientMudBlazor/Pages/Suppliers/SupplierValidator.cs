using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.General;
using Shared.Dtos.Suppliers;
using Shared.Enums.CurrencyEnums;

namespace CllientMudBlazor.Pages.Suppliers
{
    public class SupplierValidator : AbstractValidator<SupplierDto>
    {
        private IHttpServices Service;
        public SupplierValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.NickName).NotEmpty().WithMessage("Nick Name must be defined!");
            RuleFor(x => x.TaxCodeLP).NotEmpty().WithMessage("Tax Code LP must be defined!");
            RuleFor(x => x.TaxCodeLD).NotEmpty().WithMessage("Tax Code LD must be defined!");
            RuleFor(x => x.VendorCode).NotEmpty().WithMessage("Vendor Code must be defined!");
            RuleFor(x => x.SupplierCurrency).NotEqual(CurrencyEnum.None).WithMessage("Supplier currency must be defined!");
            RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail)).WithMessage("Valid email must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

            RuleFor(x => x.VendorCode).MustAsync(ReviewIfVendorCodeExist)
              .When(x => !string.IsNullOrEmpty(x.VendorCode))
              .WithMessage(x => $"{x.VendorCode} already exist");

            RuleFor(x => x.NickName).MustAsync(ReviewIfNickNameExist)
             .When(x => !string.IsNullOrEmpty(x.NickName))
             .WithMessage(x => $"{x.NickName} already exist");

        }
        async Task<bool> ReviewIfNameExist(SupplierDto request, string name, CancellationToken cancellationToken)
        {
            ValidateSupplierName validate = new()
            {
                Name = request.Name,
                Id = request.Id,

            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
        async Task<bool> ReviewIfVendorCodeExist(SupplierDto request, string name, CancellationToken cancellationToken)
        {
            ValidateSupplierVendorCode validate = new()
            {
              
                VendorCode = request.VendorCode,
                Id = request.Id,
            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
        async Task<bool> ReviewIfNickNameExist(SupplierDto request, string name, CancellationToken cancellationToken)
        {
            ValidateSupplierNickName validate = new()
            {
              
                NickName = request.NickName,
                Id = request.Id,
            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
    }
}
