using BlazingRecept.Server.Entities.Bases;

namespace BlazingRecept.Server.Entities;

public sealed class Recipe : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int PortionAmount { get; set; } = -1;
    public Guid CategoryId { get; set; } = Guid.Empty;
    public Category Category { get; set; } = new();
    public List<IngredientMeasurement> IngredientMeasurements { get; set; } = new();
}
