
using CllientMudBlazor.Templates;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.StakeHolders;
using Shared.Enums.CurrencyEnums;

namespace CllientMudBlazor.Pages.StakeHolders
{
    public partial class StakeHolders
    {
        List<StakeHolderDto> Items = new();
        string nameFilter = string.Empty;
        Func<StakeHolderDto, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
           x.Email.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
           x.PhoneNumber.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
           x.Area!.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ;

        public List<StakeHolderDto> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
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
            var result = await HttpService.PostAsync<GetAllStakeHolders, GeneralDto<List<StakeHolderDto>>>(new GetAllStakeHolders());
            if (result.Succeeded)
            {
                Items = result.Data;
                StateHasChanged();
            }
        }
        async Task Add()
        {
            CreateStakeHolder dto = new();
            var parameters = new DialogParameters<StakeHolderDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<StakeHolderDialog>("Add StakeHolder", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(StakeHolderDto dto)
        {
            EditStakeHolder request = new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Area = dto.Area,
            };
            var parameters = new DialogParameters<StakeHolderDialog>
            {

                { x => x.Model, request},
            };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


            var dialog = await DialogService.ShowAsync<StakeHolderDialog>("Edit StakeHolder", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(StakeHolderDto dto)
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
                DeleteStakeHolder request = new()
                {
                    Id = dto.Id,


                };
                var resultDelete = await HttpService.PostAsync<DeleteStakeHolder, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();



                }

            }

        }
    }
}
