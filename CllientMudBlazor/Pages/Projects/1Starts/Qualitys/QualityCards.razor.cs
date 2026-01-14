using Shared.Dtos.Starts.ExpertJudgements;
using Shared.Dtos.Starts.Qualitys;

namespace CllientMudBlazor.Pages.Projects._1Starts.Qualitys
{
    public partial class QualityCards
    {
        [Parameter] public Guid ProjectId { get; set; }
        string Title => DashBoardsStartTable.Quality.GetDescription();

        private List<QualityDto> _items = new();
        private bool _loading = true;


        private QualityDto Model = null!; // Temporary copy for editing



        protected override async Task OnInitializedAsync()
        {

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;
            var request = new GetAllQualitys { ProjectId = ProjectId };
            var response = await HttpService.PostAsync<GetAllQualitys, GeneralDto<List<QualityDto>>>(request);

            if (response.Succeeded)
                _items = response.Data ?? new();

            _loading = false;
        }

        // --- Create Logic ---
        private void StartCreate()
        {


            Model = new CreateQuality
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
            var result = await HttpService.PostAsync<QualityDto, GeneralDto>(Model);
            if (result.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged();
            }
        }


        // --- Edit Logic ---
        private async Task StartEdit(QualityDto item)
        {
            var result = await HttpService.PostAsync<GetQualityById, GeneralDto<EditQuality>>(
                new GetQualityById { Id = item.Id });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }

        public async Task DeleteAsync(QualityDto dto)
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
                DeleteQuality request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,



                };
                var resultDelete = await HttpService.PostAsync<DeleteQuality, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await LoadDataAsync();
                    NotificationService.NotifyProjectsChanged();


                }

            }

        }

        // --- Delete Logic ---
        public async Task OrderUp(QualityDto dto)
        {
            ChangeOrderQuality neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderQuality, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }


        }
        public async Task OrderDown(QualityDto dto)
        {
            ChangeOrderQuality neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderQuality, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }
        }
        async Task OpenInvestmentsDialog(QualityDto item)
        {
            var parameters = new DialogParameters<QualityInvestmentsDialog>
        {
            { x => x.QualityId, item.Id },
            { x => x.QualityName, item.Name },
            { x => x.ProjectId, ProjectId },
            { x => x.DisableAddEdit, DisableAddEdit }
        };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

            // Al cerrar el diálogo, refrescamos la lista principal para actualizar los costos mostrados en los chips
            var dialog = await DialogService.ShowAsync<QualityInvestmentsDialog>($"Investments", parameters, options);
            var result = await dialog.Result;
            
            // Refrescamos Quality para ver los totales actualizados en las tarjetas
            await LoadDataAsync();
        }


    }

}
