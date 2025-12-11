using CllientMudBlazor.Services.Enums;
using CllientMudBlazor.Services.HttPServives;
using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.Assumptions;

namespace CllientMudBlazor.Pages.Projects.Starts.Assumptions
{
    public partial class Assumption
    {
        string Title => DashBoardsStartTable.Assumptions.GetDescription();

        [Parameter]
        public Guid ProjectId { get; set; }
       

        List<AssumptionDto> Items = new();
        string nameFilter = string.Empty;

        public List<AssumptionDto> FilteredItems =>
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
            var result = await HttpService.PostAsync<GetAllAssumptions, GeneralDto<List<AssumptionDto>>>(new GetAllAssumptions()
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
            CreateAssumption dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<AssumptionDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<AssumptionDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(AssumptionDto dto)
        {
            EditAssumption model = new()
            {
                Id = dto.Id,
                ProjectId = dto.ProjectId,
                Name = dto.Name,

            };
            var parameters = new DialogParameters<AssumptionDialog>
            {

                { x => x.Model, model},
            };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<AssumptionDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(AssumptionDto dto)
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
                DeleteAssumption request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteAssumption, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();



                }

            }

        }

        public async Task OrderUp(AssumptionDto dto)
        {
            ChangeOrderAssumption neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderAssumption, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(AssumptionDto dto)
        {
            ChangeOrderAssumption neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderAssumption, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }
        }


    }
}
