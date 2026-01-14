using Shared.Dtos.Projects.Plannings.Resources;

namespace CllientMudBlazor.Pages.Projects._2Planning.Resources
{
    public partial class Resources
    {
        string Title => DashBoardsPlanningTable.Resources.GetDescription();

        [Parameter]
        public Guid ProjectId { get; set; }
        int MinOrder => Items.Count > 0 ? Items.Min(x => x.Order) : 0;
        int MaxOrder => Items.Count > 0 ? Items.Max(x => x.Order) : 0;

        List<ResourcesNeededDto> Items = new();
        string nameFilter = string.Empty;

        public List<ResourcesNeededDto> FilteredItems =>
        string.IsNullOrEmpty(nameFilter)
            ? Items.OrderBy(x => x.Order).ToList()
            : Items
                .Where(x => x.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Order)
                .ToList();

        // 🔹 Sincroniza el filtro con el genérico (usando debounce en el TextField)
        private async Task OnNameFilterChanged(string value)
        {
            nameFilter = value;
            // No necesitas StateHasChanged() aquí: FilteredItems es una propiedad calculada
            // y Blazor la reevalúa automáticamente al cambiar nameFilter.
        }

        protected override async Task OnParametersSetAsync()
        {
            await GetAll();
        }
        async Task GetAll()
        {
            var result = await HttpService.PostAsync<GetAllResourcesNeeded, GeneralDto<List<ResourcesNeededDto>>>(new GetAllResourcesNeeded()
            {
                ProjectId = ProjectId,
            });
            if (result.Succeeded)
            {
                Items = result.Data.OrderBy(x => x.Order).ToList();

                StateHasChanged();
            }
        }
        async Task Add()
        {
            CreateResourcesNeeded dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<ResourcesNeededDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<ResourcesNeededDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(ResourcesNeededDto dto)
        {
            EditResourcesNeeded model = new()
            {
                Id = dto.Id,
                ProjectId = dto.ProjectId,


            };
            var parameters = new DialogParameters<ResourcesNeededDialog>
            {

                { x => x.Model, model},
            };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


            var dialog = await DialogService.ShowAsync<ResourcesNeededDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(ResourcesNeededDto dto)
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
                DeleteResourcesNeeded request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteResourcesNeeded, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();



                }

            }

        }

        public async Task OrderUp(ResourcesNeededDto dto)
        {
            ChangeOrderResourcesNeeded neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderResourcesNeeded, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(ResourcesNeededDto dto)
        {
            ChangeOrderResourcesNeeded neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderResourcesNeeded, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }
        }


    }
}

