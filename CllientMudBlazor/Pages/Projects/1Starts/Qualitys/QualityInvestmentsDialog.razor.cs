using MudBlazor;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.Projects._1._Starts.QualityBudgetItems;
using Shared.Dtos.Starts.Qualitys;
using Shared.Enums.BudgetCategorys;

namespace CllientMudBlazor.Pages.Projects._1Starts.Qualitys
{
    public partial class QualityInvestmentsDialog
    {
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

        [Parameter] public Guid QualityId { get; set; }
        [Parameter] public string QualityName { get; set; } = string.Empty;
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public bool DisableAddEdit { get; set; }

        private List<QualityBudgetItemDto> _items = new();
        private List<BudgetItemDto> _availableBudgets = new();
        private bool _loading = true;
        private QualityBudgetItemDto Model = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;

            // 1. Cargar relaciones actuales (Muchos a Muchos)
            // Asumiendo que tienes un endpoint para obtener los items vinculados a una Quality
            var responseRel = await HttpService.PostAsync<GetAllQualityBudgetItem, GeneralDto<List<QualityBudgetItemDto>>>(
                new GetAllQualityBudgetItem { QualityId = QualityId });

            // 2. Cargar todos los BudgetItems del proyecto para el selector
          

            if (responseRel.Succeeded)
                _items = responseRel.Data ?? new();

            

            _loading = false;
        }

        private void StartCreate()
        {
            Model = new CreateQualityBudgetItem
            {
                Id = Guid.Empty,
                QualityId = QualityId,
                ProjectId = ProjectId,
                Quantity = 1,
                Category = BudgetCategory.Testing // Valor por defecto
            };
        }

        private void Cancel()
        {
            Model = null!;
        }

        private async Task Submit()
        {
           
               
            var response = await HttpService.PostAsync<QualityBudgetItemDto, GeneralDto>(Model);

            if (response.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged(); // Para refrescar totales en otras pantallas
            }
        }
        private async Task StartEdit(QualityBudgetItemDto item)
        {
            var result = await HttpService.PostAsync<GetByIdQualityBudgetItem, GeneralDto<EditQualityBudgetItem>>(
                new GetByIdQualityBudgetItem { QualityId = item.QualityId,BudgetItemId=item.BudgetItemId });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }
        private async Task DeleteAsync(QualityBudgetItemDto item)
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
                var request = new DeleteQualityBudgetItem
                {
                    QualityId = QualityId,
                    BudgetItemId = item.BudgetItemId,
                    ProjectId = ProjectId
                };

                var response = await HttpService.PostAsync<DeleteQualityBudgetItem, GeneralDto>(request);
                if (response.Succeeded)
                {
                    await LoadDataAsync();
                }
            }
        }

        private void Close() => MudDialog.Close(DialogResult.Ok(true));
    }
}
