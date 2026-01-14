using FluentValidation;
using Shared.Dtos.Plannings.RiskMatrixs;

namespace CllientMudBlazor.Pages.Projects.Planning.RiskMatrixs
{
    public class RiskMatrixValidator : AbstractValidator<RiskMatrixDto>

    {
        public RiskMatrixValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title must be defined!");
            RuleFor(x => x.Cause).NotEmpty().WithMessage("Cause must be defined!");
            RuleFor(x => x.Effect).NotEmpty().WithMessage("Effect must be defined!");

            RuleFor(x => x.Probability).NotEqual(RiskProbability.None).WithMessage("Probability must be defined!");
            RuleFor(x => x.Impact).NotEqual(RiskImpact.None).WithMessage("Impact must be defined!");

            RuleFor(x => x.StrategyType).NotEqual(RiskStrategyType.None).WithMessage("Strategy must be defined!");

            RuleFor(x => x.Responsible).NotEmpty().WithMessage("Responsible must be defined!");
     

        }
    }
}
