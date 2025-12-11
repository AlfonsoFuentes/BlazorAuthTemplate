using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Projects;
using Shared.Enums.CostCenter;
using Shared.Enums.Focuses;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages.Projects.Managements.Create
{
    public class ApproveProjectValidator : AbstractValidator<ApproveProject>
    {
        private IHttpServices Service;

        public ApproveProjectValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.ProjectNeedType).NotEqual(ProjectNeedTypeEnum.None).WithMessage("Type must be defined!");
            RuleFor(x => x.CostCenter).NotEqual(CostCenterEnum.None).WithMessage("Cost Center must be defined!");
            RuleFor(x => x.Focus).NotEqual(FocusEnum.None).WithMessage("Focus must be defined!");

            RuleFor(x => x.ProjectNumber).Matches("^[0-9]*$").WithMessage("Project Number must be number!");
            RuleFor(x => x.ProjectNumber).NotNull().WithMessage("Project Number must be defined");
            RuleFor(x => x.ProjectNumber).NotEmpty().WithMessage("Project Number must be defined");
            RuleFor(x => x.ProjectNumber).Length(5).WithMessage("Project Number must have 5 digits");
            RuleFor(x => x.ProjectNumber).MustAsync(ReviewIfProjectNumberExist)
               .When(x => !string.IsNullOrEmpty(x.ProjectNumber))
               .WithMessage(x => $"{x.ProjectNumber} already exist");
            RuleFor(x => x.ProjectName).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.ProjectName))
                .WithMessage(x => $"{x.ProjectName} already exist");
            RuleFor(x => x.PercentageContingency).GreaterThan(0).WithMessage("%Contingency must be defined!");
            RuleFor(x => x.PercentageEngineering).GreaterThan(0).WithMessage("%Engineering must be defined!");
            RuleFor(x => x.InitialProjectDate).NotNull().WithMessage("Initial project date must be defined!");
            RuleFor(x => x.BudgetItems)
               .NotEqual(0)
        
                  .WithMessage("Budget items must be defined!");
            RuleFor(x => x.PercentageTaxProductive).GreaterThan(0).When(x => x.IsProductiveAsset == false).WithMessage("%Tax (VAT) must be defined!");

            RuleFor(x => x.Stakeholders)
               .NotEqual(0)
               
                  .WithMessage("Stakeholders items must be defined!");
            RuleFor(x => x.Requirements)
             .NotEqual(0)

                .WithMessage("Requirements items must be defined!");
            RuleFor(x => x.Objectives)
             .NotEqual(0)

                .WithMessage("Objectives items must be defined!");
            RuleFor(x => x.Scopes)
             .NotEqual(0)

                .WithMessage("Scopes items must be defined!");
            RuleFor(x => x.AcceptenceCriterias)
             .NotEqual(0)

                .WithMessage("Acceptence Criterias items must be defined!");
            RuleFor(x => x.Backgrounds)
             .NotEqual(0)

                .WithMessage("Backgrounds items must be defined!");
        }


        async Task<bool> ReviewIfNameExist(ApproveProject request, string name, CancellationToken cancellationToken)
        {
            ValidateProjectName validate = new()
            {
                Name = request.ProjectName,
                Id = request.Id,


            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
        async Task<bool> ReviewIfProjectNumberExist(ApproveProject request, string name, CancellationToken cancellationToken)
        {
            ValidateProjectNumber validate = new()
            {
                ProjectNumber = request.ProjectNumber,

                Id = request.Id,

            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
    }
}
