using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Suppliers;

namespace CllientMudBlazor.Pages.Suppliers
{
    public partial class SupplierDialog
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
            var result = await HttpService.PostAsync<GetSupplierById, GeneralDto<EditSupplier>>(new GetSupplierById { Id = Model.Id });
            if (result.Succeeded)
            {
                Model = result.Data;
            }
        }

        private async Task Submit()
        {
            GeneralDto result = new();
            result = await HttpService.PostAsync<SupplierDto, GeneralDto>(Model);



            if (result.Succeeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }


        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public SupplierDto Model { get; set; } = new();
    }
}
