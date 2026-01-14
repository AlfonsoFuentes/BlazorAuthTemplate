using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Projects._2._Plannings.BudgetItemGanttTasks;

namespace CllientMudBlazor.Pages.Projects._2Planning.BudgetItemGantt
{
    public class BudgetItemGanttTaskValidator : AbstractValidator<BudgetItemGanttTaskDto>
    {
        public BudgetItemGanttTaskValidator()
        {
            RuleFor(x => x.BudgetItemId).NotEqual(Guid.Empty).WithMessage("Budget item must be defined!");
            RuleFor(x => x.AmountAssigned)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            // ✅ Validación puramente local y ultra rápida
            RuleFor(x => x.AmountAssigned)
             .LessThanOrEqualTo(x => x.MaxAllowedAmount) // Validar contra el cupo real
             .WithMessage(x => $"Insufficient funds. Maximum allowed: {x.MaxAllowedAmount.ToCurrencyCulture()}");
        }
    }
}
