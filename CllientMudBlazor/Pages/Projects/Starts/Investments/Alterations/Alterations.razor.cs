using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.Alterations;

namespace CllientMudBlazor.Pages.Projects.Starts.Investments.Alterations
{
    public partial class Alterations
    {
        string Title => "Alterations";

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

        [Parameter]
        public List<AlterationDto> _elements { get; set; } = [];


        private MudDataGrid<AlterationDto> _elementGrid = default!;
        private async Task Add()
        {
            CreateAlteration dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<AlterationDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<AlterationDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll(); 
                
                StateHasChanged();
            }
        }

        public async Task Edit(AlterationDto dto)
        {
            EditAlteration model = new()
            {

                Id = dto.Id,
            };
            var parameters = new DialogParameters<AlterationDialog>
            {
                { x => x.Model, model},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<AlterationDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
               
                StateHasChanged();
            }
        }
        private async Task Delete(AlterationDto dto)
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
                DeleteAlteration request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteAlteration, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();
                    


                }

            }

        }
        public async Task OrderUp(AlterationDto dto)
        {
            ChangeOrderAlteration neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderAlteration, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(AlterationDto dto)
        {
            ChangeOrderAlteration neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderAlteration, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }
        }


    }

}

