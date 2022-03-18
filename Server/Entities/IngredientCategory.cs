using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Entities;

public class IngredientCategory : BaseEntity
{
    public string Name { get; set; }
    public int SortOrder { get; set; }
}
