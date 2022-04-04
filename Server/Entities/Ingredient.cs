using BlazingRecept.Server.Entities.Bases;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazingRecept.Server.Entities;

[Table("Ingredient")]
public sealed class Ingredient : CategorizedEntity
{
    public string Name { get; set; } = string.Empty;
    public double Fat { get; set; } = -1.0;
    public double Carbohydrates { get; set; } = -1.0;
    public double Protein { get; set; } = -1.0;
    public double Calories { get; set; } = -1.0;
}
