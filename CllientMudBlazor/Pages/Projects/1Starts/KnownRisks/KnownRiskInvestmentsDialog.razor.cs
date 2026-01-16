using Shared.Dtos.BudgetItems;
using Shared.Dtos.Projects._1._Starts.KnownRiskBudgetItemDto;
using Shared.Enums.BudgetCategorys;

namespace CllientMudBlazor.Pages.Projects._1Starts.KnownRisks
{
    public partial class KnownRiskInvestmentsDialog
    {
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

        [Parameter] public Guid KnownRiskId { get; set; }
        [Parameter] public string KnownRiskName { get; set; } = string.Empty;
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public bool DisableAddEdit { get; set; }

        private List<KnownRiskBudgetItemDto> _items = new();
        private List<BudgetItemDto> _availableBudgets = new();
        private bool _loading = true;
        private KnownRiskBudgetItemDto Model = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;

            // 1. Cargar relaciones actuales (Muchos a Muchos)
            // Asumiendo que tienes un endpoint para obtener los items vinculados a una KnownRisk
            var responseRel = await HttpService.PostAsync<GetAllKnownRiskBudgetItem, GeneralDto<List<KnownRiskBudgetItemDto>>>(
                new GetAllKnownRiskBudgetItem { KnownRiskId = KnownRiskId });

            // 2. Cargar todos los BudgetItems del proyecto para el selector


            if (responseRel.Succeeded)
                _items = responseRel.Data ?? new();



            _loading = false;
        }

        private void StartCreate(BudgetCategory cat)
        {
            Model = new CreateKnownRiskBudgetItem
            { Id = Guid.Empty, ProjectId = ProjectId, Category = cat, Quantity = 1, KnownRiskId = KnownRiskId };
        }

        private void Cancel()
        {
            Model = null!;
        }
        private List<BudgetCategory> _allowedCategories = Enum.GetValues(typeof(BudgetCategory))
         .Cast<BudgetCategory>()
         .Where(c => c != BudgetCategory.Tax && c != BudgetCategory.Engineering && c != BudgetCategory.Contingency)
         .ToList();
        private async Task Submit()
        {


            var response = await HttpService.PostAsync<KnownRiskBudgetItemDto, GeneralDto>(Model);

            if (response.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged(); // Para refrescar totales en otras pantallas
            }
        }
        private async Task StartEdit(KnownRiskBudgetItemDto item)
        {
            var result = await HttpService.PostAsync<GetByIdKnownRiskBudgetItem, GeneralDto<EditKnownRiskBudgetItem>>(
                new GetByIdKnownRiskBudgetItem { KnownRiskId = item.KnownRiskId, BudgetItemId = item.BudgetItemId });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }
        private async Task DeleteAsync(KnownRiskBudgetItemDto item)
        {
            var parameters = new DialogParameters<DialogTemplate>
            {
                { x => x.ContentText, $"Are you sure you want to unlink this investment?" },
                { x => x.ButtonText, "Unlink" },
                { x => x.Color, Color.Error }
            };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = await DialogService.ShowAsync<DialogTemplate>("Unlink Investment", parameters, options);
            var result = await dialog.Result;

            if (!result!.Canceled)
            {
                var request = new DeleteKnownRiskBudgetItem
                {
                    KnownRiskId = KnownRiskId,
                    BudgetItemId = item.BudgetItemId,
                    ProjectId = ProjectId
                };

                var response = await HttpService.PostAsync<DeleteKnownRiskBudgetItem, GeneralDto>(request);
                if (response.Succeeded)
                {
                    await LoadDataAsync();
                }
            }
        }

        private void Close() => MudDialog.Close(DialogResult.Ok(true));
    }
}
