using Shared.Dtos.Projects;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages
{
    public partial class Home
    {
        private bool _loading = true;
        private int ActiveView = 0; // 0=Dashboard, 1=Suppliers, 2=Brands, 3=POs

        // Lista completa de proyectos para sacar métricas
        private List<ProjectDashboardDto> _projects = new();

        // Contadores KPIs
        private int _countExecution = 0;
        private int _countPlanning = 0;
        private int _countClosed = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadPortfolioData();
        }

        private async Task LoadPortfolioData()
        {
            _loading = true;
            // Usamos tu DTO existente para traer todos los proyectos
            var result = await HttpService.PostAsync<GetAllProjectDashBoards, GeneralDto<List<ProjectDashboardDto>>>(new GetAllProjectDashBoards());

            if (result.Succeeded)
            {
                _projects = result.Data ?? new();
                CalculateMetrics();
            }
            _loading = false;
        }

        private void CalculateMetrics()
        {
            // Lógica simple usando tus Enums
            _countExecution = _projects.Count(x => x.Status.Id == ProjectStatusEnum.EXECUTION_ID);
            _countPlanning = _projects.Count(x => x.Status.Id == ProjectStatusEnum.PLANNING_ID);
            _countClosed = _projects.Count(x => x.Status.Id == ProjectStatusEnum.CLOSED_ID);
        }

        // Helper para resaltar el botón activo
        private Variant GetVariant(int viewIndex) => ActiveView == viewIndex ? Variant.Filled : Variant.Text;
    }
}
