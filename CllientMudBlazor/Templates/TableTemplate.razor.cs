using Microsoft.AspNetCore.Components;
using MudBlazor;

using System.Threading.Tasks;

namespace CllientMudBlazor.Templates;
public partial class TableTemplate<TValue>
{
    [Parameter]
    public string TableTitle { get; set; } = string.Empty;

    [Parameter]
    public List<TValue> Items { get; set; } = new();

    [Parameter]
    public string NameFilter { get; set; } = string.Empty;
    [Parameter]
    public EventCallback<string> NameFilterChanged { get; set; }
    async Task OnNameFilter(string namefilter)
    {
        NameFilter = namefilter;
        await NameFilterChanged.InvokeAsync(NameFilter);
    }
    [Parameter]
    public EventCallback ExportExcel { get; set; }
    [Parameter]
    public EventCallback ExportPDF { get; set; }
    [Parameter]
    public bool ShowOrderUpDown { get; set; } = false;
    [Parameter]
    public EventCallback OrderUp { get; set; }
    [Parameter]
    public EventCallback OrderDown { get; set; }

    [Parameter]
    public bool DisableAdd {  get; set; } = false;
    [Parameter]
    public bool DisableDeleteGroup { get; set; } = false;
    [Parameter]
    public EventCallback AddNew { get; set; }
    [Parameter]
    public EventCallback DeleteGroup { get; set; }
    [Parameter]
    public EventCallback CopyGroup { get; set; }
    [Parameter]
    public EventCallback PasteGroup { get; set; }

    [Parameter]
    public RenderFragment ColumnsTemplate { get; set; } = null!;
    [Parameter]
    public HashSet<TValue> SelectedItems { get; set; } = null!;
    [Parameter]
    public EventCallback<HashSet<TValue>> SelectedItemsChanged { get; set; }
    [Parameter]
    public RenderFragment Buttons { get; set; } = null!;
    [Parameter]
    public RenderFragment OtherButtons { get; set; } = null!;
    [Parameter]
    public RenderFragment<CellContext<TValue>> ChildRowContent { get; set; } = null!;
    [Parameter]
    public EventCallback<TValue> OnRowClicked { get; set; }
    [Parameter]
    public EventCallback<TValue> OnRowRightClicked { get; set; }
    async Task RowClicked(DataGridRowClickEventArgs<TValue> args)
    {
        if (OnRowClicked.HasDelegate) await OnRowClicked.InvokeAsync(args.Item);
    }

    async Task RowRightClicked(DataGridRowClickEventArgs<TValue> args)
    {
        if (OnRowRightClicked.HasDelegate) await OnRowRightClicked.InvokeAsync(args.Item);
    }

    async Task OnSelectedItemsChanged(HashSet<TValue> items)
    {
        SelectedItems = items;
        await SelectedItemsChanged.InvokeAsync(SelectedItems);
    }
    [Parameter]
    public bool DisableUpButton { get; set; }
    [Parameter]
    public bool DisableDownButton { get; set; }
    [Parameter]
    public bool ShowAdd { get; set; } = true;
    [Parameter]
    public bool ShowToolbar { get; set; } = true;
    [Parameter]
    public bool ShowDelete { get; set; } = true;
    [Parameter]
    public bool ShowSearch { get; set; } = true;
    [Parameter]
    public bool ShowPrint { get; set; } = false;
    [Parameter]
    public bool ShowCopyPasteGroup { get; set; } = true;
}
