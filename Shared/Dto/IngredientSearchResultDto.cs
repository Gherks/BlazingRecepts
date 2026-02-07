namespace BlazingRecept.Shared.Dto;

public sealed class IngredientSearchResultDto
{
    public string Name { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public string NameSwedish { get; set; } = string.Empty;
    public double Fat { get; set; } = 0;
    public double Carbohydrates { get; set; } = 0;
    public double Protein { get; set; } = 0;
    public double Calories { get; set; } = 0;
}
