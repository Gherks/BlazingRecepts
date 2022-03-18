using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public class WeightedIngredientDto : DtoBase
{
    public IngredientDto Ingredient { get; set; } = new();
    public decimal Grams { get; set; } = 0;
}
