using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages.Base;

public class PageBase : ComponentBase
{
    protected internal bool IsLoading { get; set; } = false;
}
