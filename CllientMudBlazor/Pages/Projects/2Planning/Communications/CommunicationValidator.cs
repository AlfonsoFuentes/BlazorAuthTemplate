using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Enums;

namespace CllientMudBlazor.Pages.Projects.Planning.Communications
{
    public class CommunicationValidator : AbstractValidator<CommunicationDto>
    {
        private IHttpServices Service;
        public CommunicationValidator(IHttpServices service)
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            //RuleFor(x => x.ReceiverIds).NotEmpty().WithMessage("At least one receiver is required.");

            // --- REGLAS CONDICIONALES ---

            // 1. Si el Trigger es "Periodic" (Calendario), NO debe haber Tarea vinculada
            // y la frecuencia debe ser positiva.
            When(x => x.Trigger == CommunicationTrigger.Periodic, () =>
            {
                RuleFor(x => x.DaysOffsetOrFrequency)
                    .GreaterThan(0)
                    .WithMessage("Frequency must be greater than 0 days.");

                // Opcional: asegurar que TaskId sea null, aunque no daña si viene.
            });
            RuleFor(x => x.Type)
            .NotEqual(ActionCategory.None)
            .WithMessage("Method is required.");

            RuleFor(x => x.Artifact)
                .NotEqual(ArtifactType.None)
                .WithMessage("Artifact format is required.");

            // Validación condicional extra (Consistencia)
            //RuleFor(x => x.Artifact)
               
            //    .When(x => x.Type != ActionCategory.None && x.Artifact != ArtifactType.None)
            //    .WithMessage("This Artifact is not valid for the selected Method.");
            // 2. Si el Trigger es Basado en Tarea (Start/End/Active), 
            // ES OBLIGATORIO tener una LinkedGanttTaskId.
            When(x => x.Trigger != CommunicationTrigger.Periodic, () =>
            {
                RuleFor(x => x.SelectedGanttTask)
                    .NotNull() // No puede ser nulo
                    .WithMessage("A Gantt Task must be selected.");
            });
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
             .When(x => !string.IsNullOrEmpty(x.Name))
             .WithMessage(x => $"{x.Name} already exist");

            RuleFor(x=>x.Receivers.Count).NotEqual(0).WithMessage("Audience must be selected for this communication.");
            Service = service;
        }
       
        async Task<bool> ReviewIfNameExist(CommunicationDto request, string name, CancellationToken cancellationToken)
        {
            ValidateCommunicationName validate = new()
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
