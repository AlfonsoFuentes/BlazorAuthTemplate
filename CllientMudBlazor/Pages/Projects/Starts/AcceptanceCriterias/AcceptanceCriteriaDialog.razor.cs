using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.AcceptanceCriterias;

namespace CllientMudBlazor.Pages.Projects.Starts.AcceptanceCriterias
{
    public partial class AcceptanceCriteriaDialog
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
            var result = await HttpService.PostAsync<GetAcceptanceCriteriaById, GeneralDto<EditAcceptanceCriteria>>(new GetAcceptanceCriteriaById { Id = Model.Id });
            if (result.Suceeded)
            {
                Model = result.Data;
            }
        }

        private async Task Submit()
        {
            GeneralDto result = new();
            result = await HttpService.PostAsync<AcceptanceCriteriaDto, GeneralDto>(Model);



            if (result.Suceeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }


        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public AcceptanceCriteriaDto Model { get; set; } = new();
    }
}
