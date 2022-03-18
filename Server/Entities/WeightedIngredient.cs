using Receptacle.Server.Entities.Base;

namespace Receptacle.Server.Entities;

public class WeightedIngredient : BaseEntity
{
    public Ingredient Ingredient { get; set; }
    public int Grams { get; set; }
}
