using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.ProjectDefinitions;

public class ProjectDefinitionValidator : AbstractValidator<ProjectDefinitionItemDto>
{
    private readonly IHttpServices _service;

    public ProjectDefinitionValidator(IHttpServices service)
    {
        _service = service;

        // ✅ English Messages
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(300).WithMessage("Name cannot exceed 300 characters.");

        // ✅ Async Unique Check
        RuleFor(x => x.Name)
            .MustAsync(async (model, name, cancellation) =>
            {
                return await CheckNameUnique(model, name);
            })
            .When(x => !string.IsNullOrEmpty(x.Name)) // Only check if user typed something
            .WithMessage(x => $"'{x.Name}' already exists in this list.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }

    // Helper to call your Backend Validation Endpoint
    private async Task<bool> CheckNameUnique(ProjectDefinitionItemDto model, string name)
    {
        var request = new ValidateProjectDefinitionName
        {
            Id = model.Id,         // Guid.Empty if new
            ProjectId = model.ProjectId,
            Type = model.Type,     // Important: Validates against its own type
            Name = name
        };

        // Returns TRUE if valid (unique), FALSE if exists
        return await _service.PostForValidationAsync(request);
    }

    // Helper to bridge FluentValidation with MudBlazor
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ProjectDefinitionItemDto>.CreateWithOptions((ProjectDefinitionItemDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}