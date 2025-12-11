using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Starts.Pipes;

namespace CllientMudBlazor.Pages.Projects.Starts.Pipes
{
    public class PipeValidator   :AbstractValidator<PipeDto>
    {
        private IHttpServices Service;
        public PipeValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
             RuleFor(x => x.BudgetUSD).GreaterThanOrEqualTo(0).WithMessage("Budget must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(PipeDto request, string name, CancellationToken cancellationToken)
        {
            ValidatePipeName validate = new()
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
