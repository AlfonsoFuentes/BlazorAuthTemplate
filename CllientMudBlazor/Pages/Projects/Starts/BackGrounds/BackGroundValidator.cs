using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.BackGrounds;

namespace CllientMudBlazor.Pages.Projects.Starts.BackGrounds
{
    public class BackGroundValidator   :AbstractValidator<BackGroundDto>
    {
        private IHttpServices Service;
        public BackGroundValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(BackGroundDto request, string name, CancellationToken cancellationToken)
        {
            ValidateBackGroundName validate = new()
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
