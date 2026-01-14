using CllientMudBlazor.Pages.Projects._1Starts.StakeHolderInsideProjects;
using Shared.Dtos.Projects._2._Plannings.Communications;
using Shared.Dtos.Projects._2._Plannings.Gantts;
using Shared.Dtos.Projects.Plannings.Gantts;
using Shared.Dtos.Starts.StakeHolderInsideProjectInsideProjects;
using Shared.Enums;

namespace CllientMudBlazor.Pages.Projects._2Planning.Communications
{
    public partial class CommunicationDialog
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter] public CommunicationDto Model { get; set; } = new();
        [Parameter] public GanttDto? FixedTask { get; set; }

        // Variables de Estado
        private List<GanttDto> allTasks = new();
        private List<StakeHolderSimpleDto> allStakeholders = new();

 
        private bool IsPeriodic { get; set; } = true;
        private StakeHolderSimpleDto? SelectedStakeholderToAdd { get; set; } // Para el Autocomplete

        protected override async Task OnInitializedAsync()
        {
            // 1. Cargar Stakeholders
            await GetAllStakeHolders();

            // 2. Configurar Contexto (Tarea vs Global)
            if (FixedTask != null)
            {
                if (Model.Id == Guid.Empty)
                {
                    IsPeriodic = false;
              
                    Model.Trigger = CommunicationTrigger.TaskEnd;
                    Model.SelectedGanttTask = FixedTask;
                }
            }
            else
            {
                // Autocomplete de Tareas (Modo Global)
                var tasksResponse = await HttpService.PostAsync<GetAllGanttTasks, GeneralDto<List<GanttDto>>>(new(Model.ProjectId));
                if (tasksResponse.Succeeded) allTasks = tasksResponse.Data.RecalculateAllTasks() ?? new();
            }

            // 3. Modo Edición
            if (Model.Id != Guid.Empty)
            {
                var result = await HttpService.PostAsync<GetCommunicationById, GeneralDto<UpdateCommunication>>(new GetCommunicationById { Id = Model.Id });

                if (result.Succeeded && result.Data != null)
                {
                    Model = result.Data;
                    IsPeriodic = Model.Trigger == CommunicationTrigger.Periodic;

                    if (Model.SelectedGanttTask != null && FixedTask == null)
                    {
                        Model.SelectedGanttTask.WbsCode = allTasks.FirstOrDefault(t => t.Id == Model.SelectedGanttTask.Id)!.WbsCode;
                    }
                }
            }
        }

        async Task GetAllStakeHolders()
        {
            var result = await HttpService.PostAsync<GetAllStakeHolderInsideProjects, GeneralDto<List<StakeHolderInsideProjectDto>>>(new GetAllStakeHolderInsideProjects()
            {
                ProjectId = Model.ProjectId,
            });

            if (result.Succeeded)
            {
                allStakeholders = result.Data
                    .Select(x => new StakeHolderSimpleDto()
                    {
                        Id = x.StakeHolderId, // Ajustado a tu DTO real
                        Name = x.Name,
                        Role = x.Role.Name
                    }).ToList();
            }
        }

        // --- Search & Add Stakeholders ---

        private Task<IEnumerable<StakeHolderSimpleDto>> SearchStakeHolder(string value, CancellationToken token)
        {
            if (string.IsNullOrEmpty(value))
                return Task.FromResult<IEnumerable<StakeHolderSimpleDto>>(allStakeholders);

            return Task.FromResult(allStakeholders.Where(x =>
                x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
                x.Role.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            ));
        }

        private void OnStakeholderSelected(StakeHolderSimpleDto selected)
        {
            if (selected != null)
            {
                if (!Model.Receivers.Any(x => x.Id == selected.Id))
                {
                    Model.Receivers.Add(selected);
                }
                SelectedStakeholderToAdd = null;
            }
        }

        async Task AddStakeHolder()
        {
            CreateStakeHolderInsideProject dto = new() { ProjectId = Model.ProjectId };

            var parameters = new DialogParameters<StakeHolderInsideProjectDialog> { { x => x.Model, dto } };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<StakeHolderInsideProjectDialog>("Add Project Stakeholder", parameters, options);
            var result = await dialog.Result;

            if (result != null && !result.Canceled)
            {
                await GetAllStakeHolders();
                StateHasChanged();
            }
        }

        private void RemoveReceiver(StakeHolderSimpleDto item)
        {
            Model.Receivers.Remove(item);
        }

        // --- Tareas ---

        private async Task<IEnumerable<GanttDto>> SearchTasks(string? value, CancellationToken token)
        {
            if (string.IsNullOrEmpty(value)) return allTasks;

            // Busca si el texto coincide con el Nombre O con el WBS (Ej: "1.2")
            return allTasks.Where(x =>
                x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
                (x.WbsCode != null && x.WbsCode.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            );
        }

        // --- Submit ---

        private async Task Submit()
        {
            if (IsPeriodic)
            {
                Model.Trigger = CommunicationTrigger.Periodic;
                Model.SelectedGanttTask = null;
            }
           

            var result = await HttpService.PostAsync<CommunicationDto, GeneralDto>(Model);

            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
            }
        }

        private void Cancel() => MudDialog.Cancel();

        // ... métodos existentes ...

        // --- UI HELPERS FOR COMMUNICATION TYPE ---

       
        private void OnTypeChanged()
        {
            // 1. El @bind-Value ya actualizó el modelo, pero aseguramos la lógica
           

            // 2. Validar coherencia: Si el Artefacto actual no pega con el nuevo Tipo, lo reseteamos.
            // Por ejemplo: Si cambio a "Interactive", pero tenía seleccionado "Email", eso está mal.
            if (!IsArtifactCompatible(Model.Type, Model.Artifact))
            {
                // Reseteamos el artefacto para obligar al usuario a elegir uno válido
                Model.Artifact = 0; // O un valor default
            }
        }

        // Reglas de compatibilidad (Matriz de validación)
        private bool IsArtifactCompatible(ActionCategory type, ArtifactType artifact)
        {
            // Siempre permitir "None" para poder limpiar el campo
            if (artifact == ArtifactType.None) return true;

            return type switch
            {
                // Send: Solo cosas que se pueden enviar/adjuntar
                ActionCategory.Send => artifact is ArtifactType.Report
                                                or ArtifactType.Email
                                                or ArtifactType.Presentation,

                // Meet: Solo interacciones humanas
                ActionCategory.Meet => artifact is ArtifactType.Meeting
                                                or ArtifactType.Call,

                // Update: Solo sistemas o tableros
                ActionCategory.Update => artifact is ArtifactType.Dashboard
                                                 or ArtifactType.TaskBoard,

                // Si no hay acción seleccionada, no debería haber artefacto (salvo None)
                _ => false
            };
        }

        private IEnumerable<ArtifactType> AvailableArtifacts
        {
            get
            {
                return Model.Type switch
                {
                    // Send -> Mostrar Reporte, Email, Presentación
                    ActionCategory.Send => new[]
                    {
                ArtifactType.Report,
                ArtifactType.Email,
                ArtifactType.Presentation
            },

                    // Meet -> Mostrar Reunión, Llamada
                    ActionCategory.Meet => new[]
                    {
                ArtifactType.Meeting,
                ArtifactType.Call
            },

                    // Update -> Mostrar Dashboard, Tablero de Tareas
                    ActionCategory.Update => new[]
                    {
                ArtifactType.Dashboard,
                ArtifactType.TaskBoard
            },

                    // Si la Acción es "None", devolvemos lista vacía para bloquear el segundo select
                    _ => Array.Empty<ArtifactType>()
                };
            }
        }
        private IEnumerable<CommunicationTrigger> AvailableTriggers
        {
            get
            {
                // CASO A: El usuario marcó "Es Periódico" (Sin tarea específica)
                // La comunicación se rige por el calendario (ej: Todos los viernes).
                if (IsPeriodic)
                {
                    return new[] { CommunicationTrigger.Periodic };
                }

                // CASO B: El usuario desmarcó "Es Periódico" y seleccionó una Tarea
                // La comunicación se rige por el ESTADO de la tarea.
                return new[]
                {
            CommunicationTrigger.TaskStart,       // "Al iniciar"
            CommunicationTrigger.TaskEnd,         // "Al terminar"
            CommunicationTrigger.WhileTaskActive  // "Mientras esté activa"
        };
            }
        }

        // ✅ TASK 3: Diccionario de Frecuencias Humanas
        private Dictionary<int, string> Frequencies = new()
    {
        { 7, "Weekly (Every 7 days)" },
        { 15, "Bi-Weekly (Every 15 days)" },
        { 30, "Monthly (Every 30 days)" },
        { 90, "Quarterly (Every 3 months)" },
        { 180, "Semiannually (Every 6 months)" },
        { 365, "Annually (Every year)" }
    };
        private int SelectedFrequency
        {
            get => Frequencies.ContainsKey(Model.DaysOffsetOrFrequency) ? Model.DaysOffsetOrFrequency : 0;
            set
            {
                // Al elegir del combo, actualizamos el modelo. Si es 0 (Custom), no tocamos nada (se llena en el input)
                if (value != 0) Model.DaysOffsetOrFrequency = value;
            }
        }
    }
}

