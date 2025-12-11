using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.Electricals;

namespace CllientMudBlazor.Pages.Projects.Starts.Electricals
{
    public class ElectricalValidator   :AbstractValidator<ElectricalDto>
    {
        private IHttpServices Service;
        public ElectricalValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
             RuleFor(x => x.BudgetUSD).GreaterThanOrEqualTo(0).WithMessage("Budget must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(ElectricalDto request, string name, CancellationToken cancellationToken)
        {
            ValidateElectricalName validate = new()
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
