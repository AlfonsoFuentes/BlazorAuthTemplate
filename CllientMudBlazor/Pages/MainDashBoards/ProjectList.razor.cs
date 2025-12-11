using CllientMudBlazor.Pages.Projects.Managements.Create;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Enums.ProjectNeedTypes;

namespace CllientMudBlazor.Pages.MainDashBoards
{
    public partial class ProjectList    :IDisposable
    {
      

        public List<ProjectDashboardDto> OrderedProjects => GetOrderedProjects();
       

        protected override async Task OnInitializedAsync()
        {
            NotificationService.OnProjectsChanged += HandleProjectsChanged;
            await GetAllProjectDashBoard();
        }


        private void HandleProjectClick(ProjectDashboardDto project)
            => NavManager.NavigateTo($"/projectDashboard/{project.Id}");

        async Task CreateNewProject()
        {

            CreateProject dto = new();
            var parameters = new DialogParameters<CreateProjectDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<CreateProjectDialog>("Create Project", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAllProjectDashBoard();
                StateHasChanged();
            }
        }
        private List<ProjectDashboardDto> GetOrderedProjects()
        {
            return _projects
                .OrderByDescending(p => p.Status.Id == ProjectStatusEnum.EXECUTION_ID) // Ejecución primero
                .ThenByDescending(p => p.Status.Id == ProjectStatusEnum.PLANNING_ID)   // Luego Planning
                .ThenByDescending(p => p.Status.Id == ProjectStatusEnum.CREATED_ID)   // Luego Created
                .ThenBy(p => p.Name) // Alfabético como desempate
                .ToList();
        }
        private async Task HandleProjectsChanged()
        {
            await InvokeAsync(async () =>
            {
                try
                {
                    await GetAllProjectDashBoard(); // tu método actual
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    // Opcional: notificar al usuario (si tienes SnackbarProvider inyectado)
                     _snackbarService.ShowError($"Error loading projects: {ex.Message}");
                }
            });
        }

        public void Dispose()
        {
            NotificationService.OnProjectsChanged -= HandleProjectsChanged;
        }

        async Task GetAllProjectDashBoard()
        {
            var result = await HttpService.PostAsync<GetAllProjectDashBoards, GeneralDto<List<ProjectDashboardDto>>>(new GetAllProjectDashBoards());
            if (result.Succeeded)
            {
                _projects = result.Data;
                StateHasChanged();
            }
        }
        private List<ProjectDashboardDto> _projects = new();
    }
}
