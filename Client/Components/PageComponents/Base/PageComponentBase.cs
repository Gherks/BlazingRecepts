using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.Base;

public class PageComponentBase : ComponentBase
{
    protected internal bool IsLoading { get; set; } = false;

    internal void Refresh()
    {
        StateHasChanged();
    }
}
