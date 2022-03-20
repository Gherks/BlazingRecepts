using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Utilities;

public partial class CollapsibleComponent : ComponentBase
{
    private string _buttonText => ShowChildContent ? ExposedButtonText : CollapsedButtonText;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string CollapsedButtonText { get; set; } = string.Empty;

    [Parameter]
    public string ExposedButtonText { get; set; } = string.Empty;

    [Parameter]
    public bool ShowChildContent { get; set; } = false;

    public void Toggle()
    {
        ShowChildContent = !ShowChildContent;
    }
}
