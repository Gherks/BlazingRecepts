using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public sealed class IngredientDto : DtoBase
{
    public string Name { get; set; } = "";
    public double Fat { get; set; } = 0;
    public double Carbohydrates { get; set; } = 0;
    public double Protein { get; set; } = 0;
    public double Calories { get; set; } = 0;
    public IngredientCategoryDto CategoryDto { get; set; } = new();
}
