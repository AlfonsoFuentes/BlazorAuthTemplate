
using Microsoft.AspNetCore.Components;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages.Projects
{
    public partial class ProjectDashBoard
    {
        [Parameter]
        public Guid Id { get; set; }
        ProjectDashboardDto _selectedProject = new ProjectDashboardDto();
        protected override async Task OnParametersSetAsync()
        {
            var result = await HttpService.PostAsync<GetProjectDashBoardById, GeneralDto<ProjectDashboardDto>>(new GetProjectDashBoardById()
            {
                Id = Id,
            });
            if (result.Suceeded)
            {
                _selectedProject = result.Data;
            }
        }
        

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
    }
}
