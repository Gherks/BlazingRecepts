using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Components.PageComponents.CreateRecipePage;

public class IngredientForm
{
    public IngredientDto IngredientDto { get; set; } = new();
    public string Measurement { get; set; } = string.Empty;
    public string Grams { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
