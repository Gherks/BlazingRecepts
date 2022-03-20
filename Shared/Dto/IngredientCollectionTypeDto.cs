namespace BlazingRecept.Shared.Dto;

public sealed class IngredientCollectionTypeDto
{
    public string Name { get; set; } = string.Empty;
    public List<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
}