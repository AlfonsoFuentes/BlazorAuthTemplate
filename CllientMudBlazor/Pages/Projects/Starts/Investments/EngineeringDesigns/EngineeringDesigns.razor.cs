using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.EngineeringDesigns;

namespace CllientMudBlazor.Pages.Projects.Starts.Investments.EngineeringDesigns
{
    public partial class EngineeringDesigns
    {
        string Title => "EngineeringDesigns";

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

        [Parameter] public List<EngineeringDesignDto> _elements { get; set; } = [];


        private MudDataGrid<EngineeringDesignDto> _elementGrid = default!;
        private async Task Add()
        {
            CreateEngineeringDesign dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<EngineeringDesignDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<EngineeringDesignDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                
                StateHasChanged();
            }
        }

        public async Task Edit(EngineeringDesignDto dto)
        {
            EditEngineeringDesign model = new()
            {

                Id = dto.Id,
            };
            var parameters = new DialogParameters<EngineeringDesignDialog>
            {
                { x => x.Model, model},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<EngineeringDesignDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
               
                StateHasChanged();
            }
        }
        private async Task Delete(EngineeringDesignDto dto)
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
                DeleteEngineeringDesign request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteEngineeringDesign, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();
                    


                }

            }

        }
        public async Task OrderUp(EngineeringDesignDto dto)
        {
            ChangeOrderEngineeringDesign neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderEngineeringDesign, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(EngineeringDesignDto dto)
        {
            ChangeOrderEngineeringDesign neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderEngineeringDesign, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await GetAll();
            }
        }


    }

}

