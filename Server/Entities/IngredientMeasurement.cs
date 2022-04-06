using BlazingRecept.Server.Entities.Bases;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Entities;

public sealed class IngredientMeasurement : BaseEntity
{
    public string Measurement { get; set; } = string.Empty;
    public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
    public int Grams { get; set; } = -1;
    public string Note { get; set; } = string.Empty;
    public int SortOrder { get; set; } = -1;
    public Guid IngredientId { get; set; } = Guid.Empty;
}
