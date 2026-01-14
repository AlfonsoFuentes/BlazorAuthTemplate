using Shared.Dtos.BudgetItems;

namespace CllientMudBlazor.Pages.Projects._1Starts.KnownRisks
{
    public partial class KnownRiskInvestmentsDialog
    {
        private async Task AddInvestment(string category)
        {
            //DialogParameters p = new();
            //DialogOptions options = new() { MaxWidth = MaxWidth.Medium, FullWidth = true };
            //IDialogReference dialog = null!;

            //// 🔥 SWITCH: Decide qué Dialogo abrir según la selección del menú
            //switch (category)
            //{
            //    case "Alteration":
            //        {
            //            var dtoAlt = new CreateAlteration { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<AlterationDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Foundation":
            //        {
            //            var dtoAlt = new CreateFoundation { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<FoundationDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Structural":
            //        {
            //            var dtoAlt = new CreateStructural { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<StructuralDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Equipment":
            //        {
            //            var dtoAlt = new CreateEquipment { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<EquipmentDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Valve":
            //        {
            //            var dtoAlt = new CreateValve { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<ValveDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Electrical":
            //        {
            //            var dtoAlt = new CreateElectrical { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<ElectricalDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Pipe":
            //        {
            //            var dtoAlt = new CreatePipe { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<PipeDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Instrument":
            //        {
            //            var dtoAlt = new CreateInstrument { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<InstrumentDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "EHS":
            //        {
            //            var dtoAlt = new CreateEHS { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<EHSDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Testing":
            //        {
            //            var dtoAlt = new CreateTesting { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<TestingDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "Painting":
            //        {
            //            var dtoAlt = new CreatePainting { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<PaintingDialog>($"Add {category}", p, options);
            //        }

            //        break;
            //    case "EngineeringDesign":
            //        {
            //            var dtoAlt = new CreateEngineeringDesign { ProjectId = ProjectId, LinkToKnownRiskId = RiskId };
            //            p.Add("Model", dtoAlt);
            //            dialog = await DialogService.ShowAsync<EngineeringDesignDialog>($"Add {category}", p, options);
            //        }

            //        break;





            //        // Agrega aquí Structural, Equipment, etc...
            //        // case "Structural": ...
            //}

            //if (dialog != null)
            //{
            //    var result = await dialog.Result;
            //    if (!result!.Canceled) await LoadItems();
            //}
        }
        private async Task EditItem(BudgetItemDto item)
        {
            //DialogParameters p = new();
            //DialogOptions options = new() { MaxWidth = MaxWidth.Small, FullWidth = true };
            //IDialogReference dialog = null!;

            //// 🔥 SWITCH: Decide qué Dialogo abrir para EDITAR según la categoría guardada
            //switch (item.CategoryName)
            //{
            //    case "Alteration":
            //        {
            //            var dtoTest = new EditAlteration { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<AlterationDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Foundation":
            //        {
            //            var dtoTest = new EditFoundation { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<FoundationDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Structural":
            //        {
            //            var dtoTest = new EditStructural { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<StructuralDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Equipment":
            //        {
            //            var dtoTest = new EditEquipment { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<EquipmentDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Valve":
            //        {
            //            var dtoTest = new EditValve { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<ValveDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Electrical":
            //        {
            //            var dtoTest = new EditElectrical { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<ElectricalDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Pipe":
            //        {
            //            var dtoTest = new EditPipe { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<PipeDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Instrument":
            //        {
            //            var dtoTest = new EditInstrument { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<InstrumentDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "EHS":
            //        {
            //            var dtoTest = new EditEHS { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<EHSDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Testing":
            //        {
            //            var dtoTest = new EditTesting { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<TestingDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;
            //    case "Painting":
            //        {
            //            var dtoTest = new EditPainting { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<PaintingDialog>($"Edit {item.CategoryName}", p, options);
            //        }

            //        break;

            //    case "EngineeringDesign":
            //        {
            //            var dtoTest = new EditEngineeringDesign { Id = item.Id, ProjectId = ProjectId };
            //            p.Add("Model", dtoTest);
            //            // Nota: Asegúrate de que tu TestingDialog soporte recibir un EditTesting o cargue por ID
            //            dialog = await DialogService.ShowAsync<EngineeringDesignDialog>($"Edit {item.CategoryName}", p, options);
            //        }
                    
            //        break;

               
            //}

            //if (dialog != null)
            //{
            //    var result = await dialog.Result;
            //    if (!result!.Canceled) await LoadItems();
            //}
        }
        private async Task DeleteInvestment(BudgetItemDto item)
        {
            //bool? confirm = await DialogService.ShowMessageBox(
            //    "Warning",
            //    $"Are you sure you want to delete {item.Name}?",
            //    yesText: "Delete", cancelText: "Cancel");

            //if (confirm == true)
            //{
            //    GeneralDto result = new();

            //    // 🔥 Switch para llamar al endpoint de borrado correcto
            //    switch (item.CategoryName)
            //    {
            //        case "Alteration":
            //            result = await HttpService.PostAsync<DeleteAlteration, GeneralDto>(new DeleteAlteration { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Foundation":
            //            result = await HttpService.PostAsync<DeleteFoundation, GeneralDto>(new DeleteFoundation { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Structural":
            //            result = await HttpService.PostAsync<DeleteStructural, GeneralDto>(new DeleteStructural { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Equipment":
            //            result = await HttpService.PostAsync<DeleteEquipment, GeneralDto>(new DeleteEquipment { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Valve":
            //            result = await HttpService.PostAsync<DeleteValve, GeneralDto>(new DeleteValve { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Electrical":
            //            result = await HttpService.PostAsync<DeleteElectrical, GeneralDto>(new DeleteElectrical { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Pipe":
            //            result = await HttpService.PostAsync<DeletePipe, GeneralDto>(new DeletePipe { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Instrument":
            //            result = await HttpService.PostAsync<DeleteInstrument, GeneralDto>(new DeleteInstrument { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Testing":
            //            result = await HttpService.PostAsync<DeleteTesting, GeneralDto>(new DeleteTesting { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "Painting":
            //            result = await HttpService.PostAsync<DeletePainting, GeneralDto>(new DeletePainting { Id = item.Id, ProjectId = ProjectId });
            //            break;
            //        case "EngineeringDesign":
            //            result = await HttpService.PostAsync<DeleteEngineeringDesign, GeneralDto>(new DeleteEngineeringDesign { Id = item.Id, ProjectId = ProjectId });
            //            break;
                   
            //            // ... Agrega los demás casos aquí ...
            //    }

            //    if (result.Succeeded)
            //    {
            //        await LoadItems();
            //    }
            //}
        }
    }
}
