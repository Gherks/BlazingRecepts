using BlazingRecept.Server.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazingRecept.Server.Entities;

public sealed class Ingredient : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    //[Column(TypeName = "decimal(18,6)")]
    public double Fat { get; set; } = -1.0;
    //[Column(TypeName = "decimal(18,6)")]
    public double Carbohydrates { get; set; } = -1.0;
    //[Column(TypeName = "decimal(18,6)")]
    public double Protein { get; set; } = -1.0;
    //[Column(TypeName = "decimal(18,6)")]
    public double Calories { get; set; } = -1.0;
    public Guid IngredientCategoryId { get; set; } = Guid.Empty;
    public IngredientCategory IngredientCategory { get; set; } = new();
}
