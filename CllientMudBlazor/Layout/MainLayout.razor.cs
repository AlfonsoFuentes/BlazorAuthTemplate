
using Shared.Dtos.Brands;
using Shared.Dtos.General;
using Shared.Dtos.Projects;

namespace CllientMudBlazor.Layout
{
    public partial class MainLayout
    {
        protected override async Task OnInitializedAsync()
        {
            await GetAllProjectDashBoard();
        }
        async Task GetAllProjectDashBoard()
        {
            var result = await HttpService.PostAsync<GetAllProjectDashBoards, GeneralDto<List<ProjectDashboardDto>>>(new GetAllProjectDashBoards());
            if (result.Suceeded)
            {
                _projects=result.Data;
                StateHasChanged();
            }
        }
        private List<ProjectDashboardDto> _projects = new();



        private void OnProjectSelected(ProjectDashboardDto project)
        {

            NavManager.NavigateTo($"/projectDashboard/{project.Id}");
        }

    }
}
