using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Entities;

public sealed class IngredientMeasurement : BaseEntity
{
    public Guid IngredientId { get; set; } = Guid.Empty;
    public Ingredient Ingredient { get; set; } = new();
    public string Measurement { get; set; } = string.Empty;
    public int Grams { get; set; } = -1;
    public string Note { get; set; } = string.Empty;
}
