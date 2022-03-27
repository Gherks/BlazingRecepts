using BlazingRecept.Server.Entities.Base;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Entities;

public sealed class IngredientMeasurement : BaseEntity
{
    public Guid IngredientId { get; set; } = Guid.Empty;
    public Ingredient Ingredient { get; set; } = new();
    public string Measurement { get; set; } = string.Empty;
    public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Gram;
    public int Grams { get; set; } = -1;
    public string Note { get; set; } = string.Empty;
    public int SortOrder { get; set; } = -1;
}
