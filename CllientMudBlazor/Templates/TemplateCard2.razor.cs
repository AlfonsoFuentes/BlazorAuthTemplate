using CllientMudBlazor.Pages.Projects._1Starts.StartTemplates;
using Shared.Interfaces;

namespace CllientMudBlazor.Templates
{
    public partial class TemplateCard2<TValue> where TValue : IModelDto
    {
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public bool DisableAddEdit { get; set; }
        [Parameter] public bool HasOrder { get; set; } = true;
        [Parameter] public string Title { get; set; } = "";
        [Parameter] public List<TValue> Items { get; set; } = new();
        [Parameter] public TValue Model { get; set; } = default!;

        private bool IsCreating => Model != null && Model.Id == Guid.Empty && Model.IsEditable;
        [Parameter] public EventCallback Create { get; set; }
        [Parameter] public EventCallback<TValue> Edit { get; set; }
        [Parameter] public EventCallback Submit { get; set; }
        [Parameter] public EventCallback<TValue> Delete { get; set; }
        [Parameter] public EventCallback<TValue> GoUp { get; set; }
        [Parameter] public EventCallback<TValue> GoDown { get; set; }
        [Parameter] public EventCallback Cancel { get; set; }

        [Parameter] public RenderFragment? CustomHeaderButtons { get; set; }
        [Parameter] public RenderFragment<TemplateDialog<TValue>.TemplateDialogContext> FormContent { get; set; } = default!;
        [Parameter] public RenderFragment<TValue> MainContent { get; set; } = default!;

        // 🔥 NUEVOS PARÁMETROS PARA AGRUPACIÓN Y ORDEN

        // 1. Funciones para validar movimiento (El padre decide si sube o baja)
        [Parameter] public Func<TValue, bool>? AllowUp { get; set; }
        [Parameter] public Func<TValue, bool>? AllowDown { get; set; }

        // 2. Función para obtener la clave de agrupación (ej: item => item.Category)
        [Parameter] public Func<TValue, object>? GroupBy { get; set; }

        // 3. Template para el encabezado del grupo
        [Parameter] public RenderFragment<TValue>? GroupHeader { get; set; }
        [Parameter] public Func<TValue, string>? OrderDisplaySelector { get; set; }
    }
}
