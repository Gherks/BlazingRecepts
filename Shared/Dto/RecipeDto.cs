using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public sealed class RecipeDto : DtoBase
{
    public string Name { get; set; } = "";
    public string Instructions { get; set; } = "";
    public int BasePortions { get; set; } = 1;
    public List<WeightedIngredientDto> WeightedIngredients { get; set; } = new();
}
