using BlazingRecept.Server.Entities.Bases;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazingRecept.Server.Entities;

public sealed class Ingredient : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public double Fat { get; set; } = -1.0;
    public double Carbohydrates { get; set; } = -1.0;
    public double Protein { get; set; } = -1.0;
    public double Calories { get; set; } = -1.0;
    public Guid CategoryId { get; set; } = Guid.Empty;
}
