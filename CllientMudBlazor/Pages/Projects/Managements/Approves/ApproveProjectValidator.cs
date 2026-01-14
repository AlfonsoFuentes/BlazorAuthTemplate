using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Projects;

namespace CllientMudBlazor.Pages.Projects.Managements.Approves
{
    public class ApproveProjectStartValidator : AbstractValidator<ApproveProjectStart>
    {
        private IHttpServices Service;

        public ApproveProjectStartValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Name must be defined!");

            
            RuleFor(x => x.ProjectName).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.ProjectName))
                .WithMessage(x => $"{x.ProjectName} already exist");
           

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

            RuleFor(x => x.InitialProjectDate)
            .NotNull()

               .WithMessage("Initial date must be defined!");
        }


        async Task<bool> ReviewIfNameExist(ApproveProjectStart request, string name, CancellationToken cancellationToken)
        {
            ValidateProjectName validate = new()
            {
                Name = request.ProjectName,
                Id = request.Id,


            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
        
    }
}
