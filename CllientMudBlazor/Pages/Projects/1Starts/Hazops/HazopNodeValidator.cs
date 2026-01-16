using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.Projects._1._Starts.Hazops;
using Shared.Enums.Hazops;

namespace CllientMudBlazor.Pages.Projects._1Starts.Hazops
{
    public class HazopNodeValidator : AbstractValidator<HazopNodeDto>
    {
        private IHttpServices Service;
        public HazopNodeValidator(IHttpServices service)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name Must be defined")
                .WithMessage("El nombre del nodo no puede exceder los 100 caracteres.");
            RuleFor(x => x.DesignIntent)
                .NotEmpty().
                WithMessage("Design intent must be defined");
            Service = service;
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
               .When(x => !string.IsNullOrEmpty(x.Name))
               .WithMessage(x => $"{x.Name} already exist");
        }
        async Task<bool> ReviewIfNameExist(HazopNodeDto request, string name, CancellationToken cancellationToken)
        {
            ValidateHazopName validate = new()
            {
                Name = request.Name,
                Id = request.Id,
                ProjectId = request.ProjectId,
             


            };
            var response = await Service.PostForValidationAsync(validate);
            return response;
        }
    }
    public class HazopDetailValidator : AbstractValidator<HazopDetailDto>
    {
        public HazopDetailValidator()
        {
            RuleFor(x => x.Parameter)
                .NotEqual(HazopParameter.None).WithMessage("Parameter must be defined");
            RuleFor(x => x.GuideWord)
                .NotEqual(HazopGuideWord.None).WithMessage("Guide Word must be defined");
            RuleFor(x => x.Deviation)
                .NotEmpty().WithMessage("Deviation must be defined");
            RuleFor(x => x.Causes).NotEmpty().WithMessage("Describe the cause in more detail.");
            RuleFor(x => x.Recommendations).NotEmpty().WithMessage("A HAZOP without recommendations is incomplete.");
            RuleFor(x => x.Consequences)
               .NotEmpty().WithMessage("Consequences must be defined");

            RuleFor(x => x.Safeguards)
     .NotEmpty().WithMessage("Safeguards must be defined");

         
        }
    }
}
