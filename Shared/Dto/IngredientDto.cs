using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public sealed class IngredientDto : DtoBase
{
    public string Name { get; set; } = string.Empty;
    public double Fat { get; set; } = 0;
    public double Carbohydrates { get; set; } = 0;
    public double Protein { get; set; } = 0;
    public double Calories { get; set; } = 0;
    public Guid CategoryId { get; set; } = Guid.Empty;
    public CategoryDto CategoryDto { get; set; } = new();
}
