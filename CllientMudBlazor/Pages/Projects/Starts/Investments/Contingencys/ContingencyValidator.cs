using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.Contingencys;

namespace CllientMudBlazor.Pages.Projects.Starts.Contingencys
{
    public class ContingencyValidator   :AbstractValidator<ContingencyDto>
    {
        private IHttpServices Service;
        public ContingencyValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
             RuleFor(x => x.BudgetUSD).GreaterThanOrEqualTo(0).WithMessage("Budget must be defined!");
            



        }
        
    }
}
