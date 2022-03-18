using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public class IngredientDto : DtoBase
{
    public IngredientCategoryDto Category { get; set; } = new();
    public string Name { get; set; } = "";
    public decimal Fat { get; set; } = 0;
    public decimal Carbohydrates { get; set; } = 0;
    public decimal Protein { get; set; } = 0;
    public decimal Calories { get; set; } = 0;
}
