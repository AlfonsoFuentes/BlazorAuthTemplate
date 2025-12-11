
using Microsoft.AspNetCore.Components;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages.Projects
{
    public partial class ProjectDashBoard : IDisposable
    {
        [Parameter]
        public Guid Id { get; set; }
        ProjectDashboardDto _selectedProject = new ProjectDashboardDto();
        protected override async Task OnParametersSetAsync()
        {
            NotificationService.OnProjectsChanged += GetProjectDashBoard;
            await GetProjectDashBoard();
        }
        async Task GetProjectDashBoard()
        {
            var result = await HttpService.PostAsync<GetProjectDashBoardById, GeneralDto<ProjectDashboardDto>>(new GetProjectDashBoardById()
            {
                Id = Id,
            });
            if (result.Succeeded)
            {
                _selectedProject = result.Data;
                StateHasChanged();
            }
        }
        public bool DisableButtonsStart => _selectedProject.Status.Id == ProjectStatusEnum.CREATED_ID ? false : true;
        // ✅ Reglas de deshabilitado (optimizadas y corregidas)
        private bool DisableStart =>
            _selectedProject.Status.Id == ProjectStatusEnum.DISCARTED_ID;

        private bool DisablePlanning =>
            _selectedProject.Status.Id is ProjectStatusEnum.CREATED_ID or ProjectStatusEnum.DISCARTED_ID;

        private bool DisableExecution =>
            _selectedProject.Status.Id is ProjectStatusEnum.CREATED_ID or ProjectStatusEnum.PLANNING_ID or ProjectStatusEnum.DISCARTED_ID;

        private bool DisableMonitoring => DisableExecution; // Mismo que Execution

        private bool DisableClosing =>
            _selectedProject.Status.Id != ProjectStatusEnum.EXECUTION_ID;

        // ✅ Estado y lógica de selección
        private int SelectedPhase { get; set; } = 0;

        private void SelectPhase(int phase)
        {
            SelectedPhase = phase;
        }
        public void Dispose()
        {
            NotificationService.OnProjectsChanged -= GetProjectDashBoard;
        }
    }
}
