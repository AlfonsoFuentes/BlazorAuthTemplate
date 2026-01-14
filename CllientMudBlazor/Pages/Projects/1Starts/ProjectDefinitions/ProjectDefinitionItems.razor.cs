using Shared.Dtos.ProjectDefinitions;
using Shared.Enums.ProjectDefinitionTypes;

namespace CllientMudBlazor.Pages.Projects._1Starts.ProjectDefinitions
{
    public partial class ProjectDefinitionItems
    {
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public ProjectDefinitionType Type { get; set; }

        private List<ProjectDefinitionItemDto> _items = new();
        private bool _loading = true;


        private ProjectDefinitionItemDto Model = null!; // Temporary copy for editing



        protected override async Task OnInitializedAsync()
        {

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;
            var request = new GetAllProjectDefinitions { ProjectId = ProjectId, Type = Type };
            var response = await HttpService.PostAsync<GetAllProjectDefinitions, GeneralDto<List<ProjectDefinitionItemDto>>>(request);

            if (response.Succeeded)
                _items = response.Data ?? new();

            _loading = false;
        }

        // --- Create Logic ---
        private void StartCreate()
        {


            Model = new CreateProjectDefinitionItem
            {
                Id = Guid.Empty,
                ProjectId = ProjectId,
                Type = Type
            };

        }

        private void Cancel()
        {
            Model = null!;
        }

        private async Task Submit()
        {
            var result = await HttpService.PostAsync<ProjectDefinitionItemDto, GeneralDto>(Model);
            if (result.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged();
            }
        }


        // --- Edit Logic ---
        private async Task StartEdit(ProjectDefinitionItemDto item)
        {
            var result = await HttpService.PostAsync<GetProjectDefinitionById, GeneralDto<EditProjectDefinitionItem>>(
                new GetProjectDefinitionById { Id = item.Id });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }

        public async Task DeleteAsync(ProjectDefinitionItemDto dto)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {dto.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteProjectDefinitionItem request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,
                    Type = Type


                };
                var resultDelete = await HttpService.PostAsync<DeleteProjectDefinitionItem, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await LoadDataAsync();
                    NotificationService.NotifyProjectsChanged();


                }

            }

        }

        // --- Delete Logic ---
        public async Task OrderUp(ProjectDefinitionItemDto dto)
        {
            ChangeOrderProjectDefinitionItem neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
                Type = Type
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderProjectDefinitionItem, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }


        }
        public async Task OrderDown(ProjectDefinitionItemDto dto)
        {
            ChangeOrderProjectDefinitionItem neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,
                Type = Type
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderProjectDefinitionItem, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }
        }
    }
}
