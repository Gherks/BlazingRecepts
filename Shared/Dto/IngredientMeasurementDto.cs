using BlazingRecept.Shared.Dto.Base;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Shared.Dto;

public sealed class IngredientMeasurementDto : DtoBase
{
    public double Measurement { get; set; } = -1;
    public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
    public double Grams { get; set; } = -1;
    public string Note { get; set; } = string.Empty;
    public int SortOrder { get; set; } = -1;
    public IngredientDto IngredientDto { get; set; } = new();
}
