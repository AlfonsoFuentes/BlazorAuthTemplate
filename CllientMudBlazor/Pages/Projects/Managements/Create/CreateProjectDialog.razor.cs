using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.Brands;
using Shared.Dtos.General;
using Shared.Dtos.Projects;

namespace CllientMudBlazor.Pages.Projects.Managements.Create
{
    public partial class CreateProjectDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;
        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }


        FluentValidationValidator _fluentValidationValidator = null!;
       

        private async Task Submit()
        {
            GeneralDto result = new();
            result = await HttpService.PostAsync<CreateProject, GeneralDto>(Model);



            if (result.Succeeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }


        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public CreateProject Model { get; set; } = new();
    }
}
