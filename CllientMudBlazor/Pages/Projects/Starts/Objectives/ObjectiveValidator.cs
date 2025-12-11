using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Starts.Objectives;

namespace CllientMudBlazor.Pages.Projects.Starts.Objectives
{
    public class ObjectiveValidator   :AbstractValidator<ObjectiveDto>
    {
        private IHttpServices Service;
        public ObjectiveValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }
        async Task<bool> ReviewIfNameExist(ObjectiveDto request, string name, CancellationToken cancellationToken)
        {
            ValidateObjectiveName validate = new()
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
