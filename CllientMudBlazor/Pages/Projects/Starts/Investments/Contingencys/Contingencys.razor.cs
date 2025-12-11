using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.Contingencys;

namespace CllientMudBlazor.Pages.Projects.Starts.Investments.Contingencys
{
    public partial class Contingencys
    {
        string Title => "Contingencys";

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
        public List<ContingencyDto> _elements { get; set; } = [];


        private MudDataGrid<ContingencyDto> _elementGrid = default!;


        public async Task Edit(ContingencyDto dto)
        {
            EditContingency model = new()
            {

                Id = dto.Id,
            };
            var parameters = new DialogParameters<ContingencyDialog>
            {
                { x => x.Model, model},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<ContingencyDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                
                StateHasChanged();
            }
        }


    }
}

