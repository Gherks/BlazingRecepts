using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Utilities;

public partial class ProcessingButton : ComponentBase
{
    private bool _isProcessing = false;

    public bool IsProcessing
    {
        get
        {
            return _isProcessing;
        }

        set
        {
            _isProcessing = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public string EditFormId { get; set; } = string.Empty;

    [Parameter]
    public string CssClass { get; set; } = "btn btn-primary";

    [Parameter]
    public string Type { get; set; } = "submit";
}
