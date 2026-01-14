using Shared.Dtos.Plannings.RiskMatrixs;

namespace CllientMudBlazor.Pages.Projects._1Starts.RiskMatrixs
{
    public partial class RiskMatrix
    {
        string Title => DashBoardsStartTable.RiskMatrix.GetDescription();

        [Parameter]
        public Guid ProjectId { get; set; }
        int MinOrder => Items.Count > 0 ? Items.Min(x => x.Order) : 0;
        int MaxOrder => Items.Count > 0 ? Items.Max(x => x.Order) : 0;

        List<RiskMatrixDto> Items = new();
        string nameFilter = string.Empty;

        public List<RiskMatrixDto> FilteredItems =>
        string.IsNullOrEmpty(nameFilter)
            ? Items.OrderBy(x => x.Order).ToList()
            : Items
                .Where(x => x.Cause.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
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
            var result = await HttpService.PostAsync<GetAllRiskMatrixs, GeneralDto<List<RiskMatrixDto>>>(new GetAllRiskMatrixs()
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
            CreateRiskMatrix dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<RiskMatrixDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<RiskMatrixDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(RiskMatrixDto dto)
        {
            EditRiskMatrix model = new()
            {
                Id = dto.Id,
                ProjectId = dto.ProjectId,


            };
            var parameters = new DialogParameters<RiskMatrixDialog>
            {

                { x => x.Model, model},
            };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<RiskMatrixDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(RiskMatrixDto dto)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {dto.Title}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteRiskMatrix request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteRiskMatrix, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();



                }

            }

        }

        public async Task OrderUp(RiskMatrixDto dto)
        {
            ChangeOrderRiskMatrix neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderRiskMatrix, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(RiskMatrixDto dto)
        {
            ChangeOrderRiskMatrix neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderRiskMatrix, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }
        }


    }
}

