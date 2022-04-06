using BlazingRecept.Shared.Dto.Base;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Shared.Dto;

public sealed class IngredientMeasurementDto : DtoBase
{
    public string Measurement { get; set; } = string.Empty;
    public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
    public int Grams { get; set; } = -1;
    public string Note { get; set; } = string.Empty;
    public int SortOrder { get; set; } = -1;
    public IngredientDto IngredientDto { get; set; } = new();
}
