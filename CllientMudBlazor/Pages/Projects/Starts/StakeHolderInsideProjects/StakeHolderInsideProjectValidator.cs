using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.Requirements;
using Shared.Dtos.Starts.StakeHolderInsideProjectInsideProjects;
using Shared.Enums.StakeHolderTypes;

namespace CllientMudBlazor.Pages.Projects.Starts.Requirements
{
    public class StakeHolderInsideProjectValidator : AbstractValidator<StakeHolderInsideProjectDto>
    {
        private IHttpServices Service;
        public StakeHolderInsideProjectValidator(IHttpServices service)
        {
            Service = service;

            RuleFor(x => x.Role).NotEqual(StakeHolderRoleEnum.None).WithMessage("Role must be defined!");

            RuleFor(x => x.StakeHolderInsideProject).NotNull().WithMessage("StakeHolder must be defined!");
            RuleFor(x => x.StakeHolderInsideProject).MustAsync(ReviewIfNameExist)
                .When(x => x.StakeHolderInsideProject != null)
                .WithMessage(x => $"{x.StakeHolderInsideProject!.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(StakeHolderInsideProjectDto request, StakeHolderDto? dto, CancellationToken cancellationToken)
        {
            ValidateStakeHolderInsideProjectName validate = new()
            {
                StakeHolderId = request.StakeHolderId,
                Id = request.Id,
                ProjectId = request.ProjectId,


            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
    }
}
