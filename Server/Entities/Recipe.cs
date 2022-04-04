using BlazingRecept.Server.Entities.Bases;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazingRecept.Server.Entities;

[Table("Recipe")]
public sealed class Recipe : CategorizedEntity
{
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int PortionAmount { get; set; } = -1;
    public List<IngredientMeasurement> IngredientMeasurements { get; set; } = new();
}
