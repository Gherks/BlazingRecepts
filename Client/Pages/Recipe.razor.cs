using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Server.Controllers;

public partial class Recipe : ComponentBase
{
    [Parameter]
    public Guid Id { get; set; }
}
