
using CllientMudBlazor.Templates;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Suppliers;
using Shared.Enums.CurrencyEnums;

namespace CllientMudBlazor.Pages.Suppliers
{
    public partial class Suppliers
    {
        List<SupplierDto> Items = new();
        string nameFilter = string.Empty;
        Func<SupplierDto, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
           x.NickName.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
           x.VendorCode.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
           x.ContactName!.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
           x.ContactEmail!.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);

        public List<SupplierDto> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();
        async Task OnNameFilter(string namefilter)
        {
            nameFilter = namefilter;
            await Task.Delay(1);
        }
        protected override async Task OnInitializedAsync()
        {

            await GetAll();

        }
        async Task GetAll()
        {
            var result = await HttpService.PostAsync<GetAllSuppliers, GeneralDto<List<SupplierDto>>>(new GetAllSuppliers());
            if (result.Succeeded)
            {
                Items = result.Data;
                StateHasChanged();
            }
        }
        async Task Add()
        {
            CreateSupplier dto = new();
            var parameters = new DialogParameters<SupplierDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<SupplierDialog>("Supplier", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(SupplierDto dto)
        {
            var parameters = new DialogParameters<SupplierDialog>
        {

            { x => x.Model, dto},
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<SupplierDialog>("Supplier", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(SupplierDto dto)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {dto.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteSupplier request = new()
                {
                    Id = dto.Id,


                };
                var resultDelete = await HttpService.PostAsync<DeleteSupplier, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();



                }

            }

        }
    }
}
