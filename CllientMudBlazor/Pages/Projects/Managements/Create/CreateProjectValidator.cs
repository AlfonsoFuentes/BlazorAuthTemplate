using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Projects;
using Shared.Enums.CostCenter;
using Shared.Enums.Focuses;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages.Projects.Managements.Create
{
    public class CreateProjectValidator : AbstractValidator<CreateProject>
    {
        private IHttpServices Service;

        public CreateProjectValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.ProjectNeedType).NotEqual(ProjectNeedTypeEnum.None).WithMessage("Type must be defined!");
            RuleFor(x => x.CostCenter).NotEqual(CostCenterEnum.None).WithMessage("Cost Center must be defined!");
            RuleFor(x => x.Focus).NotEqual(FocusEnum.None).WithMessage("Focus must be defined!");



            RuleFor(x => x.ProjectName).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.ProjectName))
                .WithMessage(x => $"{x.ProjectName} already exist");
            RuleFor(x => x.PercentageContingency).GreaterThan(0).WithMessage("%Contingency must be defined!");
            RuleFor(x => x.PercentageEngineering).GreaterThan(0).WithMessage("%Engineering must be defined!");
            RuleFor(x => x.PercentageTaxProductive).GreaterThan(0).When(x => x.IsProductiveAsset == false).WithMessage("%Tax (VAT) must be defined!");
        }


        async Task<bool> ReviewIfNameExist(CreateProject request, string name, CancellationToken cancellationToken)
        {
            ValidateProjectName validate = new()
            {
                Name = request.ProjectName,


            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
    }
}
