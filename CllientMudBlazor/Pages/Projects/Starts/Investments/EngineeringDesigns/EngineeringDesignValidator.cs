using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.EngineeringDesigns;

namespace CllientMudBlazor.Pages.Projects.Starts.EngineeringDesigns
{
    public class EngineeringDesignValidator   :AbstractValidator<EngineeringDesignDto>
    {
        private IHttpServices Service;
        public EngineeringDesignValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
             RuleFor(x => x.BudgetUSD).GreaterThanOrEqualTo(0).WithMessage("Budget must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(EngineeringDesignDto request, string name, CancellationToken cancellationToken)
        {
            ValidateEngineeringDesignName validate = new()
            {
                Name = request.Name,
                Id = request.Id,
                ProjectId = request.ProjectId,
               

            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
    }
}
