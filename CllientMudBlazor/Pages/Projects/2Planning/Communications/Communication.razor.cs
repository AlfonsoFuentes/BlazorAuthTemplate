using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Enums;

namespace CllientMudBlazor.Pages.Projects._2Planning.Communications
{
    public partial class Communication
    {
     

            [Parameter] public Guid ProjectId { get; set; }
            [Parameter] public bool DisableAddEdit { get; set; } = false;
            [Parameter] public EventCallback HideTable { get; set; }

            private List<CommunicationDto> Communications = new();
            private bool IsLoading = true;

            // Propiedades calculadas
            private IEnumerable<CommunicationDto> PeriodicItems => Communications.Where(x => x.Trigger == CommunicationTrigger.Periodic);
            private IEnumerable<CommunicationDto> TaskBasedItems => Communications.Where(x => x.Trigger != CommunicationTrigger.Periodic);

            protected override async Task OnInitializedAsync()
            {
                await LoadData();
            }

            private async Task LoadData()
            {
                IsLoading = true;
                var response = await HttpService.PostAsync<GetAllProjectCommunications, GeneralDto<List<CommunicationDto>>>(new(ProjectId));

                if (response.Succeeded)
                {
                    Communications = response.Data ?? new();
                
                }
                IsLoading = false;
            }

           
            // --- CRUD ACTIONS ---

            private async Task AddNew()
            {
                // EL TRUCO: Pasamos CreateCommunication explícitamente
                CreateCommunication model = new() { ProjectId = ProjectId };

                var parameters = new DialogParameters<CommunicationDialog>
            {
                { x => x.Model, model }
            };
                var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

                var dialog = await DialogService.ShowAsync<CommunicationDialog>("Plan New Communication", parameters, options);
                var result = await dialog.Result;

                if (result != null && !result.Canceled)
                {
                    await LoadData();
                }
            }

            private async Task Edit(CommunicationDto dto)
            {
                var parameters = new DialogParameters<CommunicationDialog>
            {
                { x => x.Model, dto }
            };

                var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

                var dialog = await DialogService.ShowAsync<CommunicationDialog>("Edit Communication Plan", parameters, options);
                var result = await dialog.Result;

                if (result != null && !result.Canceled)
                {
                    await LoadData();
                }
            }

            private async Task Delete(CommunicationDto dto)
            {
                var parameters = new DialogParameters<DialogTemplate>
            {
                { x => x.ContentText, $"Are you sure you want to remove the '{dto.Name}' plan?" },
                { x => x.ButtonText, "Delete" },
                { x => x.Color, Color.Error }
            };

                var dialog = await DialogService.ShowAsync<DialogTemplate>("Confirm Delete", parameters);
                var result = await dialog.Result;

                if (result != null && !result.Canceled)
                {
                    var deleteDto = new DeleteCommunication { Id = dto.Id, ProjectId = ProjectId };
                    var response = await HttpService.PostAsync<DeleteCommunication, GeneralDto>(deleteDto);

                    if (response.Succeeded)
                    {
                        await LoadData();
                    }
                }
            }
        }
    }