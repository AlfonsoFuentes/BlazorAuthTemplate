using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.Valves;

namespace CllientMudBlazor.Pages.Projects.Starts.Valves
{
    public class ValveValidator   :AbstractValidator<ValveDto>
    {
        private IHttpServices Service;
        public ValveValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
             RuleFor(x => x.BudgetUSD).GreaterThanOrEqualTo(0).WithMessage("Budget must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(ValveDto request, string name, CancellationToken cancellationToken)
        {
            ValidateValveName validate = new()
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
