using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Brands;

namespace CllientMudBlazor.Pages.Brands
{
    public partial class BrandDialog
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
            var result = await HttpService.PostAsync<GetBrandById, GeneralDto<EditBrand>>(new GetBrandById { Id = Model.Id });
            if (result.Suceeded)
            {
                Model = result.Data;
            }
        }

        private async Task Submit()
        {
            GeneralDto result = new();
            result = await HttpService.PostAsync<BrandDto, GeneralDto>(Model);



            if (result.Suceeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }


        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public BrandDto Model { get; set; } = new();
    }
}
