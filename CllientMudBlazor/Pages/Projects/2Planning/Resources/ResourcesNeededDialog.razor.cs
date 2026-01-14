using Shared.Dtos.Projects.Plannings.Resources;

namespace CllientMudBlazor.Pages.Projects._2Planning.Resources
{
    public partial class ResourcesNeededDialog
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
        [Parameter] public ResourcesNeededDto Model { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            // Solo carga si es edición
            if (Model.Id != Guid.Empty)
            {
                var result = await HttpService.PostAsync<GetResourcesNeededById, GeneralDto<EditResourcesNeeded>>(
                    new GetResourcesNeededById { Id = Model.Id });

                if (result.Succeeded && result.Data != null)
                {

                    Model = result.Data;
                    // Si tu EditAcceptanceCriteria tiene más props, asigna aquí
                }
            }
        }

        private async Task Submit()
        {
            var result = await HttpService.PostAsync<ResourcesNeededDto, GeneralDto>(Model);
            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
