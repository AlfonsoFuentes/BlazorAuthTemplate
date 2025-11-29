using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;

namespace CllientMudBlazor.Pages.Brands
{
    public class BrandValidator : AbstractValidator<BrandDto>
    {
        private IHttpServices Service;
        public BrandValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
           
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

          

        }
        async Task<bool> ReviewIfNameExist(BrandDto request, string name, CancellationToken cancellationToken)
        {
            ValidateBrandName validate = new()
            {
                Name = request.Name,
                Id = request.Id,

            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
       
    }
}
