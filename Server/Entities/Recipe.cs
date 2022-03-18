using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Entities;

public class Recipe : BaseEntity
{
    public string Name { get; set; }
    public string Instructions { get; set; }
    public int BasePortions { get; set; }
    public List<WeightedIngredient> WeightedIngredients { get; set; }
}
