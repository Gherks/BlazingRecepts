using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Recipe : ComponentBase
{
    [Parameter]
    public Guid Id { get; set; }
}
