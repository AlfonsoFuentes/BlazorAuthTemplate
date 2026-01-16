using Blazored.FluentValidation;
using Shared.Dtos.Projects._1._Starts.Hazops;
using Shared.Enums.Hazops;
using System.Xml.Linq;

namespace CllientMudBlazor.Pages.Projects._1Starts.Hazops
{
    public partial class HazopAnalisys
    {
        [Parameter] public Guid ProjectId { get; set; }
        private List<HazopNodeDto> _nodes = new();
        private HazopNodeDto? _selectedNode;
        bool _isEditingNode = false;


        protected override async Task OnInitializedAsync()
        {
            await LoadNodes();
        }

        private async Task LoadNodes()
        {
            var result = await HttpService.PostAsync<GetAllHazopNodes, GeneralDto<List<HazopNodeDto>>>(
                new GetAllHazopNodes { ProjectId = ProjectId });
            if (result.Succeeded) _nodes = result.Data;
        }

        private async Task OnNodeSelected(HazopNodeDto node)
        {
            if (_selectedNode != null && _selectedNode.Id == node.Id)
            {
                _selectedNode = null;
                return;
            }

            var result = await HttpService.PostAsync<GetHazopNodeById, GeneralDto<EditHazopNode>>(
                new GetHazopNodeById { Id = node.Id });

            if (result.Succeeded) _selectedNode = result.Data;
        }

        // --- Gestión de Nodos ---
        private void StartCreateNode()
        {
            _selectedNode = new CreateHazopNode { Id = Guid.Empty, ProjectId = ProjectId, Name = $"New Node {_nodes.Count + 1}" };
            _isEditingNode = true;
        }

        private async Task SaveNode()
        {
            var result = await HttpService.PostAsync<HazopNodeDto, GeneralDto<EditHazopNode>>(_selectedNode!);
            if (result.Succeeded)
            {
                _selectedNode = result.Data;
                await LoadNodes();
            }
        }
        HazopDetailDto DetailModel = null!;
        // --- Gestión de Detalles (Desviaciones) ---
        bool DisableAddDetailButton => DisableAddEdit;
        private void AddDetailRow()
        {
            if (_selectedNode == null) return;

            // 1. Creamos el objeto en memoria con un ID temporal (Guid.NewGuid)
            // No llamamos al servicio HttpService aquí.
            DetailModel = new CreateHazopDetailNode
            {
                Id = Guid.NewGuid(),
                HazopNodeId = _selectedNode.Id,
                Parameter = HazopParameter.None, // Valor inicial por defecto
                GuideWord = HazopGuideWord.None,
                ProjectId = ProjectId
            };

            _selectedNode.Details.Add(DetailModel);


        }

        private async Task SaveDetail()
        {


            var result = await HttpService.PostAsync<HazopDetailDto, GeneralDto>(DetailModel);
            if (result.Succeeded)
            {


                if (_selectedNode != null)
                {
                    DetailModel = null!;
                    var resultNode = await HttpService.PostAsync<GetHazopNodeById, GeneralDto<EditHazopNode>>(
                new GetHazopNodeById { Id = _selectedNode.Id });

                    if (resultNode.Succeeded) _selectedNode = resultNode.Data;
                }

            }
        }

        private async Task DeleteDetail(HazopDetailDto detail)
        {
            var command = new DeleteHazopDetailNode { Id = detail.Id, HazopNodeId = detail.HazopNodeId, ProjectId = ProjectId };
            var result = await HttpService.PostAsync<DeleteHazopDetailNode, GeneralDto>(command);
            if (result.Succeeded) _selectedNode?.Details.Remove(detail);
        }
        async Task DeleteNode(HazopNodeDto node)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {node.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                var command = new DeleteHazopNode { Id = node.Id, ProjectId = ProjectId };
                var resultdelete = await HttpService.PostAsync<DeleteHazopNode, GeneralDto>(command);
                if (resultdelete.Succeeded)
                {
                    _selectedNode = null;
                    await LoadNodes();
                }
            }
        }
        private FluentValidationValidator? _validator;
        private bool IsValid => !(_validator?.Validate(options => options.IncludeAllRuleSets()) ?? false);
        private void CancelEditDetail()
        {
            // Si el ID es el que generamos temporalmente y no se ha guardado en DB
            if (DetailModel != null && _selectedNode != null)
            {
                // Buscamos si existe en la lista local pero no en la DB (basado en el ID temporal)
                var itemToRemove = _selectedNode.Details.FirstOrDefault(x => x.Id == DetailModel.Id);

                // Si lo encontramos y era un CreateHazopDetailNode (nuevo), lo removemos
                if (itemToRemove != null && DetailModel is CreateHazopDetailNode)
                {
                    _selectedNode.Details.Remove(itemToRemove);
                }
            }
            DetailModel = null!;
        }
        private void EditDetail(HazopDetailDto detail)
        {

            DetailModel = new EditHazopDetailNode
            {
                Id = detail.Id,
                HazopNodeId = detail.HazopNodeId,
                Parameter = detail.Parameter,
                GuideWord = detail.GuideWord,

                Causes = detail.Causes,
                Consequences = detail.Consequences,
                Safeguards = detail.Safeguards,
                Recommendations = detail.Recommendations,
                ProjectId = detail.ProjectId
            };

        }
    }
}
