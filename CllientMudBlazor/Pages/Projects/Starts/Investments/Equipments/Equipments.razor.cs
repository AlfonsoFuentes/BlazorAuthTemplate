using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.Equipments;

namespace CllientMudBlazor.Pages.Projects.Starts.Investments.Equipments
{
    public partial class Equipments
    {
        string Title => "Equipments";

        [Parameter]
        public Guid ProjectId { get; set; }

        int MinOrder => _elements.Count > 0 ? _elements.Min(x => x.Order) : 0;
        int MaxOrder => _elements.Count > 0 ? _elements.Max(x => x.Order) : 0;
        [Parameter]
        public EventCallback RefresInvestmentData { get; set; }
        async Task GetAll()
        {
            await RefresInvestmentData.InvokeAsync();
        }

        [Parameter] public List<EquipmentDto> _elements { get; set; } = [];


        private MudDataGrid<EquipmentDto> _elementGrid = default!;
        private async Task Add()
        {
            CreateEquipment dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<EquipmentDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<EquipmentDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                
                StateHasChanged();
            }
        }

        public async Task Edit(EquipmentDto dto)
        {
            EditEquipment model = new()
            {

                Id = dto.Id,
            };
            var parameters = new DialogParameters<EquipmentDialog>
            {
                { x => x.Model, model},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<EquipmentDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                
                StateHasChanged();
            }
        }
        private async Task Delete(EquipmentDto dto)
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
                DeleteEquipment request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteEquipment, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();
                    


                }

            }

        }
        public async Task OrderUp(EquipmentDto dto)
        {
            ChangeOrderEquipment neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderEquipment, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(EquipmentDto dto)
        {
            ChangeOrderEquipment neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderEquipment, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }
        }


    }

}

