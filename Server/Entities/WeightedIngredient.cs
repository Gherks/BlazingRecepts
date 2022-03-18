using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Entities;

public sealed class WeightedIngredient : BaseEntity
{
    public Ingredient Ingredient { get; set; } = new();
    public int Grams { get; set; } = -1;
}
