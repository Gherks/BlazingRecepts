using Receptacle.Server.Entities.Base;

namespace Receptacle.Server.Entities;

public class IngredientCategory : BaseEntity
{
    public string Name { get; set; }
    public int SortOrder { get; set; }
}
