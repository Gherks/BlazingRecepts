using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Common;

public partial class LoadingSpinner : ComponentBase
{
    [Parameter]
    public string Text { get; set; } = string.Empty;
}
