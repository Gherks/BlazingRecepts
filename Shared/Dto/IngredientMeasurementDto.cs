using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public sealed class IngredientMeasurementDto : DtoBase
{
    public IngredientDto IngredientDto { get; set; } = new();
    public string Measurement { get; set; } = string.Empty;
    public int Grams { get; set; } = -1;
    public string Note { get; set; } = string.Empty;
}
