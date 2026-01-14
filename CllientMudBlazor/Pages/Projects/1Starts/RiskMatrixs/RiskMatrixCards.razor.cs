using Shared.Dtos.Plannings.RiskMatrixs;
using Shared.Dtos.Projects._1._Starts.RiskMatrixs.RiskResponseActions;
using Shared.Dtos.Starts.ExpertJudgements;

namespace CllientMudBlazor.Pages.Projects._1Starts.RiskMatrixs
{
    public partial class RiskMatrixCards
    {
        [Parameter] public Guid ProjectId { get; set; }
        string Title => DashBoardsStartTable.RiskMatrix.GetDescription();

        private List<RiskMatrixDto> _items = new();
        private bool _loading = true;


        private RiskMatrixDto Model = null!; // Temporary copy for editing



        protected override async Task OnInitializedAsync()
        {

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _loading = true;
            var request = new GetAllRiskMatrixs { ProjectId = ProjectId };
            var response = await HttpService.PostAsync<GetAllRiskMatrixs, GeneralDto<List<RiskMatrixDto>>>(request);

            if (response.Succeeded)
                _items = response.Data ?? new();

            _loading = false;
        }

        // --- Create Logic ---
        private void StartCreate()
        {


            Model = new CreateRiskMatrix
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
            var result = await HttpService.PostAsync<RiskMatrixDto, GeneralDto>(Model);
            if (result.Succeeded)
            {
                Model = null!;
                await LoadDataAsync();
                NotificationService.NotifyProjectsChanged();
            }
        }


        // --- Edit Logic ---
        private async Task StartEdit(RiskMatrixDto item)
        {
            var result = await HttpService.PostAsync<GetRiskMatrixById, GeneralDto<EditRiskMatrix>>(
                new GetRiskMatrixById { Id = item.Id });

            if (result.Succeeded && result.Data != null)
            {
                Model = result.Data;

            }

        }

        public async Task DeleteAsync(RiskMatrixDto dto)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {dto.Title}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteRiskMatrix request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,



                };
                var resultDelete = await HttpService.PostAsync<DeleteRiskMatrix, GeneralDto>(request);
                if (resultDelete.Succeeded)
                {
                    await LoadDataAsync();
                    NotificationService.NotifyProjectsChanged();


                }

            }

        }

        // --- Delete Logic ---
        public async Task OrderUp(RiskMatrixDto dto)
        {
            ChangeOrderRiskMatrix neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderRiskMatrix, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }


        }
        public async Task OrderDown(RiskMatrixDto dto)
        {
            ChangeOrderRiskMatrix neworder = new()
            {
                Id = dto.Id,

                ProjectId = ProjectId,

            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderRiskMatrix, GeneralDto>(neworder);
            if (result.Succeeded)
            {
                await LoadDataAsync();
            }
        }

        private Color GetColor(string colorString)
        {
            return colorString switch
            {
                "Color.Error" => Color.Error,
                "Color.Warning" => Color.Warning,
                "Color.Success" => Color.Success,
                _ => Color.Default
            };
        }
        private List<RiskMatrixCommentDto> _comments => Model.RiskMatrixComments ?? new();
        private string _newComment = string.Empty;
        private string _newCommentAuthor = string.Empty;

        private Guid? _editingCommentId = null;
        private string _tempCommentValue = string.Empty;
        private string _tempCommentAuthor = string.Empty;
        private async Task AddComment()
        {
            if (string.IsNullOrWhiteSpace(_newComment) || string.IsNullOrWhiteSpace(_newCommentAuthor)) return;

            if (Model.Id == Guid.Empty)
            {
                // MEMORIA: Riesgo nuevo
                var newComment = new RiskMatrixCommentDto
                {
                    Id = Guid.NewGuid(),
                    RiskMatrixId = Guid.Empty,
                    Comment = _newComment,
                    CommentedBy = _newCommentAuthor,
                    CommentDate = DateTime.Now
                };
                _comments.Add(newComment);
            }
            else
            {
                // API: Riesgo existente
                var dto = new CreateRiskMatrixComment
                {
                    RiskMatrixId = Model.Id,
                    Comment = _newComment,
                    CommentedBy = _newCommentAuthor
                };
                // Endpoint inferido: "CreateRiskMatrixComment"
                var result = await HttpService.PostAsync<CreateRiskMatrixComment, GeneralDto>(dto);
                if (result.Succeeded) await ReloadComments();
            }

            _newComment = string.Empty;
            _newCommentAuthor = string.Empty;
        }

        private async Task DeleteComment(RiskMatrixCommentDto item)
        {
            bool? confirm = await DialogService.ShowMessageBox("Delete", "Are you sure?", yesText: "Delete", cancelText: "Cancel");
            if (confirm != true) return;

            if (Model.Id == Guid.Empty)
            {
                _comments.Remove(item);
            }
            else
            {
                // Asumiendo que tienes DeleteRiskMatrixComment
                var dto = new DeleteRiskMatrixComment { Id = item.Id };
                var result = await HttpService.PostAsync<DeleteRiskMatrixComment, GeneralDto>(dto);
                if ((result.Succeeded))
                {
                    await ReloadComments();
                }

            }
        }

        private void StartEditComment(RiskMatrixCommentDto item)
        {
            _editingCommentId = item.Id;
            _tempCommentValue = item.Comment;
            _tempCommentAuthor = item.CommentedBy; // 2. Cargar el autor actual
        }

        private void CancelEditComment()
        {
            var item = _comments.FirstOrDefault(x => x.Id == _editingCommentId);
            if (item != null) item.Comment = _tempCommentValue;

            _editingCommentId = null;
            _tempCommentValue = string.Empty;
            _tempCommentAuthor = string.Empty; // 3. Limpiar variable
        }

        private async Task SaveEditComment(RiskMatrixCommentDto item)
        {
            if (Model.Id == Guid.Empty)
            {
                _editingCommentId = null;
                _newCommentAuthor = string.Empty;
                _tempCommentValue = string.Empty;
                _tempCommentAuthor = string.Empty;
            }
            else
            {
                var dto = new EditRiskMatrixComment
                {
                    Id = item.Id,
                    RiskMatrixId = Model.Id,
                    Comment = _tempCommentValue,   // Usamos el valor editado
                    CommentedBy = _tempCommentAuthor // Usamos el valor editado
                };
                // Endpoint inferido: "EditRiskMatrixComment"
                var result = await HttpService.PostAsync<EditRiskMatrixComment, GeneralDto>(dto);
                if (result.Succeeded)
                {
                    _editingCommentId = null;
                    _tempCommentValue = string.Empty;
                    _tempCommentAuthor = string.Empty; // Limpiamos
                    await ReloadComments();
                }
            }
        }

        private async Task ReloadComments()
        {

            var req = new GetRiskMatrixById { Id = Model.Id };
            var res = await HttpService.PostAsync<GetRiskMatrixById, GeneralDto<EditRiskMatrix>>(req);
            if (res.Succeeded && res.Data != null)
            {
                Model = res.Data;
            }
        }
        private RiskResponseActionDto _currentAction = new();
        private bool _isEditingAction = false;

        // Esta lista alimenta la tabla visual
        private List<RiskResponseActionDto> _actionsList => Model.RiskResponseActions ?? new();

        // --- GESTIÓN DE ACCIONES (El corazón de la lógica) ---

        private void PrepareNewAction()
        {
            _currentAction = new RiskResponseActionDto
            {
                Id = Guid.NewGuid(),
                RiskMatrixId = Model.Id, // Si es nuevo será Empty
                DueDate = DateTime.Now.AddDays(7)
            };
            _isEditingAction = false;
        }

        private void PrepareEditAction(RiskResponseActionDto item)
        {
            // Clonamos para no editar directamente en la grilla hasta guardar
            _currentAction = new RiskResponseActionDto
            {
                Id = item.Id,
                RiskMatrixId = item.RiskMatrixId,
                Description = item.Description,
                AssignedTo = item.AssignedTo,
                DueDate = item.DueDate,
                ActionType = item.ActionType,
                IsCompleted = item.IsCompleted,
                Order = item.Order
            };
            _isEditingAction = true;
        }

        private async Task SaveAction()
        {
            if (string.IsNullOrWhiteSpace(_currentAction.Description)) return; // Validación básica

            if (Model.Id == Guid.Empty)
            {
                // --- MODO MEMORIA (El padre no existe aún) ---
                if (_isEditingAction)
                {
                    // Buscamos en la lista local y reemplazamos
                    var existing = _actionsList.FirstOrDefault(x => x.Id == _currentAction.Id);
                    if (existing != null)
                    {
                        existing.Description = _currentAction.Description;
                        existing.AssignedTo = _currentAction.AssignedTo;
                        existing.DueDate = _currentAction.DueDate;
                        existing.ActionType = _currentAction.ActionType;
                    }
                }
                else
                {
                    // Agregamos a la lista local
                    _currentAction.Order = _actionsList.Count + 1; // Orden simple
                    Model.RiskResponseActions.Add(_currentAction);
                }
            }
            else
            {
                // --- MODO API (El padre ya existe) ---
                if (_isEditingAction)
                {
                    var request = new EditRiskResponseAction
                    {
                        Id = _currentAction.Id,
                        Description = _currentAction.Description,
                        AssignedTo = _currentAction.AssignedTo,
                        DueDate = _currentAction.DueDate,
                        ActionType = _currentAction.ActionType,
                        IsCompleted = _currentAction.IsCompleted,
                        RiskMatrixId = Model.Id
                    };
                    var res = await HttpService.PostAsync<EditRiskResponseAction, GeneralDto>(request);
                    if (res.Succeeded) await ReloadActions();
                }
                else
                {
                    var request = new CreateRiskResponseAction
                    {
                        RiskMatrixId = Model.Id,
                        Description = _currentAction.Description,
                        AssignedTo = _currentAction.AssignedTo,
                        DueDate = _currentAction.DueDate,
                        ActionType = _currentAction.ActionType
                        // El Backend calcula el Order
                    };
                    var res = await HttpService.PostAsync<CreateRiskResponseAction, GeneralDto>(request);
                    if (res.Succeeded) await ReloadActions();
                }
            }

            // Limpiar formulario
            PrepareNewAction();
        }

        private async Task DeleteAction(RiskResponseActionDto item)
        {
            if (Model.Id == Guid.Empty)
            {
                // MODO MEMORIA
                Model.RiskResponseActions.Remove(item);
            }
            else
            {
                // MODO API
                var request = new DeleteRiskResponseAction { Id = item.Id };
                var res = await HttpService.PostAsync<DeleteRiskResponseAction, GeneralDto>(request);
                if (res.Succeeded) await ReloadActions();
            }
        }

        // Método auxiliar para recargar solo la lista (o el padre completo si prefieres)
        private async Task ReloadActions()
        {
            var req = new GetAllRiskResponseActionsByRiskId { RiskMatrixId = Model.Id };
            var res = await HttpService.PostAsync<GetAllRiskResponseActionsByRiskId, GeneralDto<List<RiskResponseActionDto>>>(req);
            if (res.Succeeded)
            {
                Model.RiskResponseActions = res.Data;
                StateHasChanged();
            }
        }
    }
}
