using Shared.Dtos.BudgetItems;
using Shared.Dtos.Projects._1._Starts.RiskMatrixBudgetItemDto;
using Shared.Enums.BudgetCategorys;

namespace CllientMudBlazor.Pages.Projects._1Starts.RiskMatrixs
{
    public partial class RiskMatrixInvestmentsDialog
    {
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

        [Parameter] public Guid RiskMatrixId { get; set; }
        [Parameter] public string RiskMatrixName { get; set; } = string.Empty;
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public bool DisableAddEdit { get; set; }

        private List<RiskMatrixBudgetItemDto> _items = new();
        private List<BudgetItemDto> _availableBudgets = new();
        private bool _loading = true;
        private RiskMatrixBudgetItemDto Model = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;

            // 1. Cargar relaciones actuales (Muchos a Muchos)
            // Asumiendo que tienes un endpoint para obtener los items vinculados a una RiskMatrix
            var responseRel = await HttpService.PostAsync<GetAllRiskMatrixBudgetItem, GeneralDto<List<RiskMatrixBudgetItemDto>>>(
                new GetAllRiskMatrixBudgetItem { RiskMatrixId = RiskMatrixId });

            // 2. Cargar todos los BudgetItems del proyecto para el selector


            if (responseRel.Succeeded)
                _items = responseRel.Data ?? new();



            _loading = false;
        }

        private void StartCreate(BudgetCategory cat)
        {
            Model = new CreateRiskMatrixBudgetItem
            { Id = Guid.Empty, ProjectId = ProjectId, Category = cat, Quantity = 1, RiskMatrixId = RiskMatrixId };
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


            var response = await HttpService.PostAsync<RiskMatrixBudgetItemDto, GeneralDto>(Model);

            if (response.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged(); // Para refrescar totales en otras pantallas
            }
        }
        private async Task StartEdit(RiskMatrixBudgetItemDto item)
        {
            var result = await HttpService.PostAsync<GetByIdRiskMatrixBudgetItem, GeneralDto<EditRiskMatrixBudgetItem>>(
                new GetByIdRiskMatrixBudgetItem { RiskMatrixId = item.RiskMatrixId, BudgetItemId = item.BudgetItemId });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }
        private async Task DeleteAsync(RiskMatrixBudgetItemDto item)
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
                var request = new DeleteRiskMatrixBudgetItem
                {
                    RiskMatrixId = RiskMatrixId,
                    BudgetItemId = item.BudgetItemId,
                    ProjectId = ProjectId
                };

                var response = await HttpService.PostAsync<DeleteRiskMatrixBudgetItem, GeneralDto>(request);
                if (response.Succeeded)
                {
                    await LoadDataAsync();
                }
            }
        }

        private void Close() => MudDialog.Close(DialogResult.Ok(true));
    }
    }
