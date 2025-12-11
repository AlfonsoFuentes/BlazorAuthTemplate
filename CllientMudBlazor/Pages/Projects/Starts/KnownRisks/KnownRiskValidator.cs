using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.KnownRisks;

namespace CllientMudBlazor.Pages.Projects.Starts.KnownRisks
{
    public class KnownRiskValidator   :AbstractValidator<KnownRiskDto>
    {
        private IHttpServices Service;
        public KnownRiskValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(KnownRiskDto request, string name, CancellationToken cancellationToken)
        {
            ValidateKnownRiskName validate = new()
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
