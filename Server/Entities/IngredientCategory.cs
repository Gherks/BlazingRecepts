using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Entities;

public sealed class IngredientCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; } = -1;
    public List<Ingredient> Ingredients { get; set; } = new();
}
