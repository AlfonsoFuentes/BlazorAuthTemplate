using CllientMudBlazor.Services.Enums;
using CllientMudBlazor.Services.HttPServives;
using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.StakeHolderInsideProjectInsideProjects;

namespace CllientMudBlazor.Pages.Projects.Starts.StakeHolderInsideProjects
{
    public partial class StakeHolderInsideProject
    {
        string Title => DashBoardsStartTable.StakeHolders.GetDescription();

        [Parameter]
        public Guid ProjectId { get; set; }
   

        List<StakeHolderInsideProjectDto> Items = new();
        string nameFilter = string.Empty;

        public List<StakeHolderInsideProjectDto> FilteredItems =>
        string.IsNullOrEmpty(nameFilter)
            ? Items.ToList()
            : Items
                .Where(x => x.StakeHolderInsideProject!.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
         
                .ToList();

        // 🔹 Sincroniza el filtro con el genérico (usando debounce en el TextField)
        private async Task OnNameFilterChanged(string value)
        {
            nameFilter = value;
            // No necesitas StateHasChanged() aquí: FilteredItems es una propiedad calculada
            // y Blazor la reevalúa automáticamente al cambiar nameFilter.
        }

        protected override async Task OnParametersSetAsync()
        {
            await GetAll();
        }
        async Task GetAll()
        {
            var result = await HttpService.PostAsync<GetAllStakeHolderInsideProjects, GeneralDto<List<StakeHolderInsideProjectDto>>>(new GetAllStakeHolderInsideProjects()
            {
                ProjectId = ProjectId,
            });
            if (result.Succeeded)
            {
                Items = result.Data;
                
                StateHasChanged();
            }
        }
        async Task Add()
        {
            CreateStakeHolderInsideProject dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<StakeHolderInsideProjectDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<StakeHolderInsideProjectDialog>($"Add {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(StakeHolderInsideProjectDto dto)
        {
            EditStakeHolderInsideProject model = new()
            {
                Id = dto.Id,
                ProjectId = dto.ProjectId,
                StakeHolderInsideProject = dto.StakeHolderInsideProject,


            };
            var parameters = new DialogParameters<StakeHolderInsideProjectDialog>
            {

                { x => x.Model, model},
            };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


            var dialog = await DialogService.ShowAsync<StakeHolderInsideProjectDialog>($"Edit {Title}", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(StakeHolderInsideProjectDto dto)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {dto.StakeHolderInsideProject!.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteStakeHolderInsideProject request = new()
                {
                  
                    ProjectId = ProjectId,
                    StakeHolderId = dto.StakeHolderId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteStakeHolderInsideProject, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();



                }

            }

        }

       


    }
}
