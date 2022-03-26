using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Entities;

public sealed class Recipe : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int PortionAmount { get; set; } = -1;
    public List<IngredientMeasurement> IngredientMeasurements { get; set; } = new();
}
