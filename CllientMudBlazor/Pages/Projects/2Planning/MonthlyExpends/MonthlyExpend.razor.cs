using Shared.Dtos.Projects.Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.MonthlyExpends
{
    public partial class MonthlyExpend : ComponentBase
    {
        [Parameter] public Guid ProjectId { get; set; }
       

        protected MonthlyExpenditureResponse Data { get; set; } = new();
        protected bool Loading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        protected async Task LoadData()
        {
            Loading = true;
            var response = await HttpService.PostAsync<GetMonthlyExpendByProject, GeneralDto<MonthlyExpenditureResponse>>(
                new GetMonthlyExpendByProject(ProjectId));

            if (response?.Succeeded == true)
            {
                Data = response.Data ?? new();
            }
            Loading = false;
        }
    }
}
