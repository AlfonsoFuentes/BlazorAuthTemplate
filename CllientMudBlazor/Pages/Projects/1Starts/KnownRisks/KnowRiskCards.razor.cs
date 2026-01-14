using CllientMudBlazor.Pages.StakeHolders;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.ExpertJudgements;
using Shared.Dtos.Starts.KnownRisks;

namespace CllientMudBlazor.Pages.Projects._1Starts.KnownRisks
{
    public partial class KnowRiskCards
    {
        [Parameter] public Guid ProjectId { get; set; }
        string Title => DashBoardsStartTable.KnownRisks.GetDescription();

        private List<KnownRiskDto> _items = new();
        private bool _loading = true;


        private KnownRiskDto Model = null!; // Temporary copy for editing



        protected override async Task OnInitializedAsync()
        {

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;
            var request = new GetAllKnownRisks { ProjectId = ProjectId };
            var response = await HttpService.PostAsync<GetAllKnownRisks, GeneralDto<List<KnownRiskDto>>>(request);

            if (response.Succeeded)
                _items = response.Data ?? new();

            _loading = false;
        }

        // --- Create Logic ---
        private void StartCreate()
        {


            Model = new CreateKnownRisk
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
            var result = await HttpService.PostAsync<KnownRiskDto, GeneralDto>(Model);
            if (result.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged();
            }
        }


        // --- Edit Logic ---
        private async Task StartEdit(KnownRiskDto item)
        {
            var result = await HttpService.PostAsync<GetKnownRiskById, GeneralDto<EditKnownRisk>>(
                new GetKnownRiskById { Id = item.Id });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }

        public async Task DeleteAsync(KnownRiskDto dto)
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
                DeleteKnownRisk request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,



                };
                var resultDelete = await HttpService.PostAsync<DeleteKnownRisk, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await LoadDataAsync();
                    NotificationService.NotifyProjectsChanged();


                }

            }

        }

        // --- Delete Logic ---
        public async Task OrderUp(KnownRiskDto dto)
        {
            ChangeOrderKnownRisk neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderKnownRisk, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }


        }
        public async Task OrderDown(KnownRiskDto dto)
        {
            ChangeOrderKnownRisk neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderKnownRisk, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }
        }


    }
}
