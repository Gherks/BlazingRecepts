using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Common;

public partial class NoteTooltipIcon : ComponentBase
{
    [Parameter]
    public IngredientMeasurementDto? IngredientMeasurementDto { get; set; }
}
