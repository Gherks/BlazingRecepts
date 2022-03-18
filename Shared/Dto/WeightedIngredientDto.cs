using Receptacle.Shared.Dto.Base;

namespace Receptacle.Shared.Dto;

public class WeightedIngredientDto : DtoBase
{
    public IngredientDto Ingredient { get; set; } = new();
    public decimal Grams { get; set; } = 0;
}
