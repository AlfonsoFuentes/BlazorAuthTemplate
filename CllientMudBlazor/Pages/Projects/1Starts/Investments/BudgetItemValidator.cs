using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.BudgetItems;

namespace Server.EndPoints.ProjectDashBoard.ProjectStarts.Investments
{
    public class BudgetItemValidator :AbstractValidator<BudgetItemDto>
    {
        private IHttpServices Service;
        public BudgetItemValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.BudgetUSD).GreaterThan(0).WithMessage("Budget must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(BudgetItemDto request, string name, CancellationToken cancellationToken)
        {
            ValidateBudgetItemName validate = new()
            {
                Name = request.Name,
                Id = request.Id,
                ProjectId = request.ProjectId,
                Category=request.Category,


            };
            var response = await Service.PostForValidationAsync(validate);
            return response;
        }
    }
}
