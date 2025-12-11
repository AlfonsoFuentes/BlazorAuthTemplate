using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.Taxs;

namespace CllientMudBlazor.Pages.Projects.Starts.Taxs
{
    public class TaxValidator   :AbstractValidator<TaxDto>
    {
        private IHttpServices Service;
        public TaxValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
             RuleFor(x => x.BudgetUSD).GreaterThanOrEqualTo(0).WithMessage("Budget must be defined!");
           



        }
       
    }
}
