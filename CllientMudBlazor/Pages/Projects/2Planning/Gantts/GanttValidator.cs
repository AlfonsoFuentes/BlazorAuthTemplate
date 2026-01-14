using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects.Starts.AcceptanceCriterias
{
    public class GanttValidator : AbstractValidator<GanttDto>
    {
        private readonly IHttpServices _service;

        public GanttValidator(IHttpServices service)
        {
            _service = service;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage("'{PropertyValue}' already exists.");

            // ✅ Autodependencia: no necesita allTasks
          
        }

        private async Task<bool> ReviewIfNameExist(GanttDto request, string name, CancellationToken cancellationToken)
        {
            var validate = new ValidateGanttTaskName(request.ProjectId, request.Id, request.Name);
            var response = await _service.PostForValidationAsync(validate);
            return response;
        }
    }
}
