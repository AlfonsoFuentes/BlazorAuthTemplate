using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.Starts.AcceptanceCriterias;

namespace CllientMudBlazor.Pages.Projects.Starts.AcceptanceCriterias
{
    public class AcceptanceCriteriaValidator   :AbstractValidator<AcceptanceCriteriaDto>
    {
        private IHttpServices Service;
        public AcceptanceCriteriaValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(AcceptanceCriteriaDto request, string name, CancellationToken cancellationToken)
        {
            ValidateAcceptanceCriteriaName validate = new()
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
