
using CllientMudBlazor.Templates;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Brands;
using Shared.Enums.CurrencyEnums;

namespace CllientMudBlazor.Pages.Brands
{
    public partial class Brands
    {
        List<BrandDto> Items = new();
        string nameFilter = string.Empty;
        Func<BrandDto, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ;

        public List<BrandDto> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
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
            var result = await HttpService.PostAsync<GetAllBrands, GeneralDto<List<BrandDto>>>(new GetAllBrands());
            if (result.Suceeded)
            {
                Items = result.Data;
                StateHasChanged();
            }
        }
        async Task Add()
        {
            CreateBrand dto = new();
            var parameters = new DialogParameters<BrandDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<BrandDialog>("Brand", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(BrandDto dto)
        {
            var parameters = new DialogParameters<BrandDialog>
        {

            { x => x.Model, dto},
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


            var dialog = await DialogService.ShowAsync<BrandDialog>("Brand", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(BrandDto dto)
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
                DeleteBrand request = new()
                {
                    Id = dto.Id,


                };
                var resultDelete = await HttpService.PostAsync<DeleteBrand, GeneralDto>(request);
                if (resultDelete.Suceeded)
                {
                    await GetAll();



                }

            }

        }
    }
}
