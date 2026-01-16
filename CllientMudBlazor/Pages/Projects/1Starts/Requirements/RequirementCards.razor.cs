using CllientMudBlazor.Pages.Projects._1Starts.Requirements;
using CllientMudBlazor.Pages.StakeHolders;
using Shared.Dtos.ProjectDefinitions;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.Requirements;
using Shared.Dtos.Starts.Requirements;
using Shared.Enums.ProjectDefinitionTypes;

namespace CllientMudBlazor.Pages.Projects._1Starts.Requirements
{
    public partial class RequirementCards
    {
        string Title => DashBoardsStartTable.Requirements.GetDescription();
        [Parameter] public Guid ProjectId { get; set; }


        private List<RequirementDto> _items = new();
        private bool _loading = true;


        private RequirementDto Model = null!; // Temporary copy for editing



        protected override async Task OnInitializedAsync()
        {
            await GetStakeholders();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;
            var request = new GetAllRequirements { ProjectId = ProjectId };
            var response = await HttpService.PostAsync<GetAllRequirements, GeneralDto<List<RequirementDto>>>(request);

            if (response.Succeeded)
                _items = response.Data ?? new();

            _loading = false;
        }

        // --- Create Logic ---
        private void StartCreate()
        {


            Model = new CreateRequirement
            {
                Id = Guid.Empty,
                ProjectId = ProjectId,

            };

        }

        private void Cancel()
        {
            Model = null!;
        }

        private async Task Submit()
        {
            var result = await HttpService.PostAsync<RequirementDto, GeneralDto>(Model);
            if (result.Succeeded)
            {
                Model = null!;
                NotificationService.NotifyProjectsChanged();
                await LoadDataAsync();
                StateHasChanged();
            }
        }


        // --- Edit Logic ---
        private async Task StartEdit(RequirementDto item)
        {
            var result = await HttpService.PostAsync<GetRequirementById, GeneralDto<EditRequirement>>(
                new GetRequirementById { Id = item.Id });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }

        public async Task DeleteAsync(RequirementDto dto)
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
                DeleteRequirement request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,



                };
                var resultDelete = await HttpService.PostAsync<DeleteRequirement, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    NotificationService.NotifyProjectsChanged();
                    await LoadDataAsync();
                   

                    StateHasChanged();
                }

            }

        }

        // --- Delete Logic ---
        public async Task OrderUp(RequirementDto dto)
        {
            ChangeOrderRequirement neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderRequirement, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }


        }
        public async Task OrderDown(RequirementDto dto)
        {
            ChangeOrderRequirement neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderRequirement, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }
        }
        private Task<IEnumerable<StakeHolderDto>> SearchStakeHolder(string value, CancellationToken token)
        {
            Func<StakeHolderDto, bool> Criteria = x =>
            x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
            x.Area.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
            x.Email.Contains(value, StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<StakeHolderDto> FilteredItems = string.IsNullOrEmpty(value) ? stakeHolders.AsEnumerable() :
                stakeHolders.Where(Criteria);
            return Task.FromResult(FilteredItems);
        }
        void ChangeRequeridBy()
        {
            if (Model.RequestedBy != null)
            {



            }

        }
        void ChangeResponsible()
        {
            if (Model.Responsible != null)
            {



            }

        }
        public async Task AddStakeHolder()
        {
            CreateStakeHolder dto = new();
            var parameters = new DialogParameters<StakeHolderDialog>
        {
              { x => x.Model, dto},
        };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<StakeHolderDialog>("Add StakeHolder", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetStakeholders();

            }
        }
        List<StakeHolderDto> stakeHolders = new();
        async Task GetStakeholders()
        {
            var result = await HttpService.PostAsync<GetAllStakeHolders, GeneralDto<List<StakeHolderDto>>>(new GetAllStakeHolders());
            if (result.Succeeded)
            {
                stakeHolders = result.Data;
                StateHasChanged();
            }
        }
        async Task OpenInvestmentsDialog(RequirementDto item)
        {
            var parameters = new DialogParameters<RequirementInvestmentsDialog>
        {
            { x => x.RequirementId, item.Id },
            { x => x.RequirementName, item.Name },
            { x => x.ProjectId, ProjectId },
            { x => x.DisableAddEdit, DisableAddEdit }
        };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

            // Al cerrar el diálogo, refrescamos la lista principal para actualizar los costos mostrados en los chips
            var dialog = await DialogService.ShowAsync<RequirementInvestmentsDialog>($"Investments", parameters, options);
            var result = await dialog.Result;

            // Refrescamos Quality para ver los totales actualizados en las tarjetas
            await LoadDataAsync();
        }

    }
}
