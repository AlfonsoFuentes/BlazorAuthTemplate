using Shared.Dtos.Projects._2._Plannings.Gantts;

namespace CllientMudBlazor.Pages.Projects._2Planning.MonthlyExpends
{
    public partial class AssignmentDetailDialog
    {
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public Guid BudgetItemId { get; set; }
        [Parameter] public Guid ProjectId { get; set; }

        private string _budgetItemName = "";
        private List<BudgetItemAssignmentDetailDto> _details = new();
        private bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadDetails();
        }

        private async Task LoadDetails()
        {
            _loading = true;
            // Llamada al endpoint para obtener el desglose
            var response = await HttpService.PostAsync<GetBudgetItemAssignmentDetail,
                GeneralDto<List<BudgetItemAssignmentDetailDto>>>(new GetBudgetItemAssignmentDetail(BudgetItemId));

            if (response != null&&response.Succeeded) _details = response.Data;
            _loading = false;
        }

        private void Close() => MudDialog.Cancel();
    }
}
