using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Entities;

public class WeightedIngredient : BaseEntity
{
    public Ingredient Ingredient { get; set; }
    public int Grams { get; set; }
}
