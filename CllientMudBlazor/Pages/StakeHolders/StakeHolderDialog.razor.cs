using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.StakeHolders;

namespace CllientMudBlazor.Pages.StakeHolders
{
    public partial class StakeHolderDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;
        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }


        FluentValidationValidator _fluentValidationValidator = null!;
        protected override async Task OnInitializedAsync()
        {
            if (Model.Id == Guid.Empty) return;
            var result = await HttpService.PostAsync<GetStakeHolderById, GeneralDto<EditStakeHolder>>(new GetStakeHolderById { Id = Model.Id });
            if (result.Succeeded)
            {
                Model = result.Data;
            }
        }

        private async Task Submit()
        {
            GeneralDto result = new();
            result = await HttpService.PostAsync<StakeHolderDto, GeneralDto>(Model);



            if (result.Succeeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }


        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public StakeHolderDto Model { get; set; } = new();
    }
}
