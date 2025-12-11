using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.Engineerings;

namespace CllientMudBlazor.Pages.Projects.Starts.Investments.Engineerings
{
    public partial class Engineerings
    {
        string Title => "Engineerings";

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

        [Parameter] public List<EngineeringSalarysDto> _elements { get; set; } = [];


        private MudDataGrid<EngineeringSalarysDto> _elementGrid = default!;
       

        public async Task Edit(EngineeringSalarysDto dto)
        {
            EditEngineeringSalarys model = new()
            {

                Id = dto.Id,
            };
            var parameters = new DialogParameters<EngineeringDialog>
            {
                { x => x.Model, model},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<EngineeringDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
              
                StateHasChanged();
            }
        }
       


    }

}

