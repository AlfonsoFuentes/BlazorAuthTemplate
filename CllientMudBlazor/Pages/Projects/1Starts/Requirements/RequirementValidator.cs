using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.Requirements;
using Shared.Enums.RequirementPrioritys;

namespace CllientMudBlazor.Pages.Projects.Starts.Requirements
{
    public class RequirementValidator   :AbstractValidator<RequirementDto>
    {
        private IHttpServices Service;
        public RequirementValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.Responsible).NotNull().WithMessage("Responsible must be defined!");

            RuleFor(x => x.RequestedBy).NotNull().WithMessage("Requestor must be defined!");
            RuleFor(x => x.Type).NotEqual(RequirementTypeEnum.None).WithMessage("Type must be defined!");
            RuleFor(x => x.Priority).NotEqual(RequirementPriorityEnum.None).WithMessage("Priority must be defined!");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(RequirementDto request, string name, CancellationToken cancellationToken)
        {
            ValidateRequirementName validate = new()
            {
                Name = request.Name,
                Id = request.Id,
                ProjectId = request.ProjectId,
               

            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
    }
}
