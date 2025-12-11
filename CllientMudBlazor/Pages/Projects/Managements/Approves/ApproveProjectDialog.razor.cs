using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.Brands;
using Shared.Dtos.General;
using Shared.Dtos.Projects;

namespace CllientMudBlazor.Pages.Projects.Managements.Approves
{
    public partial class ApproveProjectDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;
        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }


        FluentValidationValidator _fluentValidationValidator = null!;

        override protected async Task OnInitializedAsync()
        {
            var result = await HttpService.PostAsync<GetProjectToApproveById, GeneralDto<ApproveProject>>(new GetProjectToApproveById()
            {
                Id = Model.Id
            });
            if (result.Succeeded )
            {
                Model = result.Data;
            }
        }

        private async Task Submit()
        {
            GeneralDto result = new();
            result = await HttpService.PostAsync<ApproveProject, GeneralDto>(Model);



            if (result.Succeeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }


        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public ApproveProject Model { get; set; } = new();
    }
}
