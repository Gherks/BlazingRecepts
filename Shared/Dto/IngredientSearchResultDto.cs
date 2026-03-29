namespace BlazingRecept.Shared.Dto;

public sealed class IngredientSearchResultDto
{
    public string Name { get; init; } = string.Empty;
    public string NameEnglish { get; init; } = string.Empty;
    public string NameSwedish { get; init; } = string.Empty;
    public double Fat { get; init; } = 0;
    public double Carbohydrates { get; init; } = 0;
    public double Protein { get; init; } = 0;
    public double Calories { get; init; } = 0;
}
