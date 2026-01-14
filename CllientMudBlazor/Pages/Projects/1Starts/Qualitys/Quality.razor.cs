using Shared.Dtos.Starts.Qualitys;

namespace CllientMudBlazor.Pages.Projects._1Starts.Qualitys
{
    public partial class Quality
    {
        string Title => DashBoardsStartTable.Quality.GetDescription();

        [Parameter]
        public Guid ProjectId { get; set; }
   

        List<QualityDto> Items = new();
        string nameFilter = string.Empty;

        public List<QualityDto> FilteredItems =>
        string.IsNullOrEmpty(nameFilter)
            ? Items.OrderBy(x => x.Order).ToList()
            : Items
                .Where(x => x.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Order)
                .ToList();
        int MinOrder => Items.Count > 0 ? Items.Min(x => x.Order) : 0;
        int MaxOrder => Items.Count > 0 ? Items.Max(x => x.Order) : 0;
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
            var result = await HttpService.PostAsync<GetAllQualitys, GeneralDto<List<QualityDto>>>(new GetAllQualitys()
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
            CreateQuality dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<QualityDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<QualityDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(QualityDto dto)
        {
            EditQuality model = new()
            {
                Id = dto.Id,
                ProjectId = dto.ProjectId,
                Name = dto.Name,

            };
            var parameters = new DialogParameters<QualityDialog>
            {

                { x => x.Model, model},
            };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<QualityDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(QualityDto dto)
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
                DeleteQuality request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteQuality, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();



                }

            }

        }

        public async Task OrderUp(QualityDto dto)
        {
            ChangeOrderQuality neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderQuality, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(QualityDto dto)
        {
            ChangeOrderQuality neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderQuality, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }
        }

        async Task OpenInvestmentsDialog(QualityDto item)
        {
            var parameters = new DialogParameters<QualityInvestmentsDialog>
        {
            { x => x.QualityId, item.Id },
            { x => x.QualityName, item.Name },
            { x => x.ProjectId, ProjectId },
            { x => x.DisableAddEdit, DisableAddEdit }
        };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

            // Al cerrar el diálogo, refrescamos la lista principal para actualizar los costos mostrados en los chips
            var dialog = await DialogService.ShowAsync<QualityInvestmentsDialog>($"Investments", parameters, options);
            var result = await dialog.Result;
            if(result!.Canceled)
                return;
            // Refrescamos Quality para ver los totales actualizados en las tarjetas
            await GetAll();
        }
    }
}
