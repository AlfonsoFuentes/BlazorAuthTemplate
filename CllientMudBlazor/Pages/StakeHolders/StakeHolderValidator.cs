using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.General;
using Shared.Dtos.StakeHolders;
using Shared.Enums.CurrencyEnums;

namespace CllientMudBlazor.Pages.StakeHolders
{
    public class StakeHolderValidator : AbstractValidator<StakeHolderDto>
    {
        private IHttpServices Service;
        public StakeHolderValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number must be defined!");
            RuleFor(x => x.Area).NotEmpty().WithMessage("Area must be defined!");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must be defined!");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be defined properly!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

            RuleFor(x => x.Email).MustAsync(ReviewIfEmailExist)
                .When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage(x => $"{x.Email} already exist");


        }
        async Task<bool> ReviewIfNameExist(StakeHolderDto request, string name, CancellationToken cancellationToken)
        {
            ValidateStakeHolderName validate = new()
            {
                Name = request.Name,
                Id = request.Id,

            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
        async Task<bool> ReviewIfEmailExist(StakeHolderDto request, string name, CancellationToken cancellationToken)
        {
            ValidateStakeHolderEmail validate = new()
            {
              
                Email = request.Email,
                Id = request.Id,
            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
        
    }
}
