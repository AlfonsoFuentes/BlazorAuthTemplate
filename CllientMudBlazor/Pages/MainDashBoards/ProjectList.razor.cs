using Microsoft.AspNetCore.Components;
using Shared.Dtos.Projects;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages.MainDashBoards
{
    public partial class ProjectList
    {
        [Parameter] public List<ProjectDashboardDto> Projects { get; set; } = new();

        public List<ProjectDashboardDto> OrderedProjects => GetOrderedProjects();
        [Parameter]
        public EventCallback GetAllProjectDashBoard { get; set; }



        private void HandleProjectClick(ProjectDashboardDto project)
            => NavManager.NavigateTo($"/projectDashboard/{project.Id}");

        async Task CreateNewProject()
        {

            if (GetAllProjectDashBoard.HasDelegate)
                await GetAllProjectDashBoard.InvokeAsync();
        }
        private List<ProjectDashboardDto> GetOrderedProjects()
        {
            return Projects
                .OrderByDescending(p => p.Status.Id == ProjectStatusEnum.EXECUTION_ID) // Ejecución primero
                .ThenByDescending(p => p.Status.Id == ProjectStatusEnum.PLANNING_ID)   // Luego Planning
                .ThenByDescending(p => p.Status.Id == ProjectStatusEnum.CREATED_ID)   // Luego Created
                .ThenBy(p => p.Name) // Alfabético como desempate
                .ToList();
        }
    }
}
