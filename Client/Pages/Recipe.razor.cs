using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Recipe : ComponentBase
{
    private RecipeDto? _recipeDto = new RecipeDto();

    [Parameter]
    public Guid RecipeId { get; set; }

    [Inject]
    public IRecipeService? RecipeService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (RecipeService == null) throw new InvalidOperationException();

        _recipeDto = await RecipeService.GetByIdAsync(RecipeId);
    }

    private bool ContainsInstructions()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        return string.IsNullOrWhiteSpace(_recipeDto.Instructions) == false;
    }

    private string GetMeasurement(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (ingredientMeasurementDto == null) throw new InvalidOperationException();

        return ingredientMeasurementDto.Measurement.Trim() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }
}
