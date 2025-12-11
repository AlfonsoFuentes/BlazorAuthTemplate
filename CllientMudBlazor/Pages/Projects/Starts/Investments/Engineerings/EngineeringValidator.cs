using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Starts.Engineerings;

namespace CllientMudBlazor.Pages.Projects.Starts.Engineerings
{
    public class EngineeringValidator   :AbstractValidator<EngineeringSalarysDto>
    {
        private IHttpServices Service;
        public EngineeringValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
             RuleFor(x => x.BudgetUSD).GreaterThanOrEqualTo(0).WithMessage("Budget must be defined!");
           


        }
        
    }
}
