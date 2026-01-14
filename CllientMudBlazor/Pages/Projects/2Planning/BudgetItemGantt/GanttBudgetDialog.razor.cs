using MudBlazor;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.Projects._2._Plannings.BudgetItemGanttTasks;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CllientMudBlazor.Pages.Projects._2Planning.BudgetItemGantt
{
    public partial class GanttBudgetDialog
    {
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
        [Parameter] public Guid GanttTaskId { get; set; }
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public string TaskName { get; set; } = string.Empty;
        [Parameter] public bool DisableAddEdit { get; set; }

        private List<BudgetItemGanttTaskDto> _assignments = new();
        private List<BudgetItemGanttTaskDto> _availableBudgets = new();
        private BudgetItemGanttTaskDto Model = null!;
        private bool _loading = true;

        protected override async Task OnInitializedAsync() => await LoadData();

        private async Task LoadData()
        {
            _loading = true;
            // 1. Cargar asignaciones actuales
            var res1 = await HttpService.PostAsync<GetAllBudgetItemGanttTask, GeneralDto<List<BudgetItemGanttTaskDto>>>(new(GanttTaskId, ProjectId));
            if (res1.Succeeded) _assignments = res1.Data;

            // 2. Cargar lista de disponibles para el buscador
            var res2 = await HttpService.PostAsync<GetAvailableBudgetsForGantt, GeneralDto<List<BudgetItemGanttTaskDto>>>(new(ProjectId, GanttTaskId));
            if (res2.Succeeded) _availableBudgets = res2.Data;

            _loading = false;
        }



        private string GetAvailableText(BudgetItemGanttTaskDto model) =>
            model.NewAvailableBalance.ToCurrencyCulture();

        // --- Métodos de Acción CRUD ---
        // (StartCreate, StartEdit, SubmitAssignment, DeleteAssignment llamando a tus endpoints)

        private void Close() => MudDialog.Close(DialogResult.Ok(true));
        private void StartCreate()
        {
            Model = new CreateBudgetItemGanttTask
            {
                GanttTaskId = GanttTaskId,
                ProjectId = ProjectId,

            };

        }

        private async Task StartEdit(BudgetItemGanttTaskDto item)
        {
           var result=await HttpService.PostAsync<GetBudgetItemGanttTask, GeneralDto<EditBudgetItemGanttTask>>(new(GanttTaskId, item.BudgetItemId));
            if(result.Succeeded)
            {
                Model = result.Data;
               
            }

            StateHasChanged();
        }

        private async Task SubmitAssignment()
        {
            GeneralDto response = await HttpService.PostAsync<BudgetItemGanttTaskDto, GeneralDto>(Model);



            if (response.Succeeded)
            {
                Model = null!;
                await LoadData(); // Recarga lista y saldos actualizados
            }
        }
        void ChangeBudgetItem()
        {
            if (Model?.BudgetItem != null)
            {
                var selectedFromList = _availableBudgets.FirstOrDefault(x => x.BudgetItem.Id == Model.BudgetItem.Id);
                if (selectedFromList != null)
                {
                    Model.AvailableBalance = selectedFromList.AvailableBalance;

                   
                    // ASEGÚRATE DE QUE QUEDE EN CERO PARA NUEVOS REGISTROS:
                    if (Model.Id == Guid.Empty) Model.AmountAssigned = 0;
                }
            }
        }
        private async Task DeleteAssignment(BudgetItemGanttTaskDto item)
        {
            var confirm = await DialogService.ShowMessageBox(
                "Remove Link",
                $"Are you sure you want to remove the link to '{item.BudgetName}'? The funds will return to the budget balance.",
                yesText: "Remove", noText: "Cancel");

            if (confirm == true)
            {
                var request = new DeleteBudgetItemGanttTask(item.Id, ProjectId, GanttTaskId);
                var response = await HttpService.PostAsync<DeleteBudgetItemGanttTask, GeneralDto>(request);

                if (response.Succeeded)
                {

                    await LoadData();
                }
            }
        }

        private async Task<IEnumerable<BudgetItemDto>> SearchBudgetItems(string value, CancellationToken token)
        {
            if (string.IsNullOrEmpty(value))
                return _availableBudgets.Select(x => x.BudgetItem).OrderBy(x=>x.Nomenclatore).AsEnumerable();

            return _availableBudgets.Where(x =>
                x.BudgetName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
                x.Nomenclatore.Contains(value, StringComparison.InvariantCultureIgnoreCase)).Select(x => x.BudgetItem).OrderBy(x => x.Nomenclatore).AsEnumerable();
        }

        private void CancelEdit() => Model = null!;
    }
}
