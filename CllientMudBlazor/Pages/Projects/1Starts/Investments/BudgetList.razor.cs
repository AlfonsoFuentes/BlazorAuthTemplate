using MudBlazor;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.ProjectDefinitions;
using Shared.Enums.BudgetCategorys;

namespace CllientMudBlazor.Pages.Projects._1Starts.Investments
{
    public partial class BudgetList
    {
        [Parameter] public bool DisableAddEdit { get; set; }
        [Parameter] public Guid ProjectId { get; set; }


        private List<BudgetItemDto> _items = new();
        private bool _loading = true;
        private BudgetItemDto Model = null!;

        private List<BudgetCategory> _allowedCategories = Enum.GetValues(typeof(BudgetCategory))
           .Cast<BudgetCategory>()
           .Where(c => c != BudgetCategory.Tax && c != BudgetCategory.Engineering && c != BudgetCategory.Contingency)
           .ToList();

        protected override async Task OnInitializedAsync() => await LoadDataAsync();

        private async Task LoadDataAsync()
        {
            _loading = true;
            var request = new GetAllBudgetItems { ProjectId = ProjectId };
            var response = await HttpService.PostAsync<GetAllBudgetItems, GeneralDto<List<BudgetItemDto>>>(request);
            if (response.Succeeded) _items = response.Data ?? new();
            _loading = false;

            CalculateTotals();
        }

        // 🔥 LOGICA CRÍTICA: Control de Flechas por Categoría

        private bool CheckAllowUp(BudgetItemDto item)
        {
            // 1. Calculados no se mueven
            if (item.Id == Guid.Empty) return false;

            // 2. Solo sube si su Orden es > 1 (dentro de su categoría)
            return item.Order > 1;
        }

        private bool CheckAllowDown(BudgetItemDto item)
        {
            // 1. Calculados no se mueven
            if (item.Id == Guid.Empty) return false;

            // 2. Buscamos el MaxOrder SOLO de su categoría
            //    (Usamos LINQ en memoria sobre _items que ya tenemos cargado)
            var maxOrderInCategory = _items
                .Where(x => x.Category == item.Category && x.Id != Guid.Empty)
                .Max(x => x.Order);

            // 3. Solo baja si es menor que el máximo de SU grupo
            return item.Order < maxOrderInCategory;
        }

        // Resto de métodos (StartCreate, Submit, Delete, etc.) igual que antes...
        private void StartCreate(BudgetCategory cat) { Model = new CreateBudgetItem { Id = Guid.Empty, ProjectId = ProjectId, Category = cat, Quantity = 1 }; }
        private async Task StartEdit(BudgetItemDto item)
        {
            var result = await HttpService.PostAsync<GetBudgetItemById, GeneralDto<EditBudgetItem>>(new GetBudgetItemById { Id = item.Id });

            if (result.Succeeded) Model = result.Data;
        }
        private void Cancel() => Model = null!;
        private async Task Submit()
        {
            var res = await HttpService.PostAsync<BudgetItemDto, GeneralDto>(Model);
            if (res.Succeeded)
            {
                Model = null!; await LoadDataAsync();
                NotificationService.NotifyProjectsChanged();
            }
        }
        private async Task DeleteAsync(BudgetItemDto item) {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {item.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteBudgetItem request = new()
                {
                    Id = item.Id,
                    ProjectId = ProjectId,
                    


                };
                var resultDelete = await HttpService.PostAsync<DeleteBudgetItem, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await LoadDataAsync();
                    NotificationService.NotifyProjectsChanged();


                }

            }
        }

        // 🔥 IMPORTANTE: ChangeOrder debe usar el endpoint que acepta Category
        private async Task OrderUp(BudgetItemDto item) => await ChangeOrder(item, -1);
        private async Task OrderDown(BudgetItemDto item) => await ChangeOrder(item, 1);

        private async Task ChangeOrder(BudgetItemDto item, int dir)
        {
            var req = new ChangeOrderBudgetItem { Id = item.Id, ProjectId = ProjectId, NewOrder = item.Order + dir, Category = item.Category };
            var res = await HttpService.PostAsync<ChangeOrderBudgetItem, GeneralDto>(req);
            if (res.Succeeded) await LoadDataAsync();
        }
        private decimal _totalCapital = 0;
        private decimal _totalExpenses = 0;
        private decimal _totalAppropriations => _totalCapital + _totalExpenses;
        private void CalculateTotals()
        {
            // 1. Calcular Capital (Suma todo lo que NO sea gasto)
            _totalCapital = _items
                .Where(x => x.IsCapital)
                .Sum(x => x.BudgetUSD);

            // 2. Calcular Expenses (Solo lo que sea gasto)
            _totalExpenses = _items
                .Where(x => x.IsExpense)
                .Sum(x => x.BudgetUSD);
        }
    }
}
