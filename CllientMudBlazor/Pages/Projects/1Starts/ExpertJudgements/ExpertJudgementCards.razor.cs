using CllientMudBlazor.Pages.StakeHolders;
using Shared.Dtos.ProjectDefinitions;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.ExpertJudgements;
using Shared.Enums.ProjectDefinitionTypes;

namespace CllientMudBlazor.Pages.Projects._1Starts.ExpertJudgements
{
    public partial class ExpertJudgementCards
    {
        [Parameter] public Guid ProjectId { get; set; }


        private List<ExpertJudgementDto> _items = new();
        private bool _loading = true;


        private ExpertJudgementDto Model = null!; // Temporary copy for editing



        protected override async Task OnInitializedAsync()
        {
            await GetStakeholders();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;
            var request = new GetAllExpertJudgements { ProjectId = ProjectId };
            var response = await HttpService.PostAsync<GetAllExpertJudgements, GeneralDto<List<ExpertJudgementDto>>>(request);

            if (response.Succeeded)
                _items = response.Data ?? new();

            _loading = false;
        }

        // --- Create Logic ---
        private void StartCreate()
        {


            Model = new CreateExpertJudgement
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
            var result = await HttpService.PostAsync<ExpertJudgementDto, GeneralDto>(Model);
            if (result.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged();
            }
        }


        // --- Edit Logic ---
        private async Task StartEdit(ExpertJudgementDto item)
        {
            var result = await HttpService.PostAsync<GetExpertJudgementById, GeneralDto<EditExpertJudgement>>(
                new GetExpertJudgementById { Id = item.Id });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }

        public async Task DeleteAsync(ExpertJudgementDto dto)
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
                DeleteExpertJudgement request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,



                };
                var resultDelete = await HttpService.PostAsync<DeleteExpertJudgement, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await LoadDataAsync();
                    NotificationService.NotifyProjectsChanged();


                }

            }

        }

        // --- Delete Logic ---
        public async Task OrderUp(ExpertJudgementDto dto)
        {
            ChangeOrderExpertJudgement neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderExpertJudgement, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }


        }
        public async Task OrderDown(ExpertJudgementDto dto)
        {
            ChangeOrderExpertJudgement neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderExpertJudgement, GeneralDto>(neworder);
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
        void ChangeStakeHolder()
        {
            if (Model.Expert != null)
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
    }
}
