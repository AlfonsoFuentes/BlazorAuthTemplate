using FluentValidation;
using Shared.Dtos.Projects.Plannings.Resources;

namespace CllientMudBlazor.Pages.Projects.Planning.Resources
{
    public class ResourceNeededValidator    :AbstractValidator<ResourcesNeededDto>
    {
        public ResourceNeededValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.Speciality).NotEmpty().WithMessage("Speciality must be defined!");
        }
    }
}
