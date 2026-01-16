using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.Projects._1._Starts.RequirementBudgetItemDto;

namespace CllientMudBlazor.Pages.Projects.Starts.Requirements
{
    public class RequirementBudgetItemDtoValidator : AbstractValidator<RequirementBudgetItemDto>
    {
        private IHttpServices Service;
        public RequirementBudgetItemDtoValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.RequirementId).NotEqual(Guid.Empty).WithMessage("Model is null!");
            RuleFor(x => x.BudgetName).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.BudgetUSD).GreaterThan(0).WithMessage("Budget must be defined!");
            RuleFor(x => x.BudgetName).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.BudgetName))
                .WithMessage(x => $"{x.BudgetName} already exist");



        }
        async Task<bool> ReviewIfNameExist(RequirementBudgetItemDto request, string name, CancellationToken cancellationToken)
        {
            ValidateBudgetItemName validate = new()
            {
                Name = request.BudgetName,
                Id = request.BudgetItemId,
                ProjectId = request.ProjectId,
                Category = request.Category,


            };
            var response = await Service.PostForValidationAsync(validate);
            return response;
        }
    }
}
