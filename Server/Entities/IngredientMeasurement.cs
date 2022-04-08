using BlazingRecept.Server.Entities.Bases;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Entities;

public sealed class IngredientMeasurement : BaseEntity
{
    public double Measurement { get; set; } = -1;
    public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Unassigned;
    public double Grams { get; set; } = -1;
    public string Note { get; set; } = string.Empty;
    public int SortOrder { get; set; } = -1;
    public Guid IngredientId { get; set; } = Guid.Empty;
}
