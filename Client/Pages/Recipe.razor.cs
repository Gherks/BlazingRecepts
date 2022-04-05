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

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (RecipeService == null) throw new InvalidOperationException();

        _recipeDto = await RecipeService.GetByIdAsync(RecipeId);
    }

    private void HandleNavigationToEditRecipe(RecipeDto recipeDto)
    {
        if (NavigationManager == null) throw new InvalidOperationException();

        NavigationManager.NavigateTo($"recipeworkbench/{recipeDto.Id}");
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
    private double GetTotalGrams()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        double grams = _recipeDto.IngredientMeasurementDtos.Sum(ingredientMeasurementDto => ingredientMeasurementDto.Grams);

        return Math.Round(grams);
    }

    private double GetTotalFat()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        double totalFat = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in _recipeDto.IngredientMeasurementDtos)
        {
            totalFat += ingredientMeasurementDto.IngredientDto.Fat * ingredientMeasurementDto.IngredientDto.Fat;
        }

        return Math.Round(totalFat);
    }

    private double GetTotalCarbohydrates()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        double totalCarbohydrates = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in _recipeDto.IngredientMeasurementDtos)
        {
            totalCarbohydrates += ingredientMeasurementDto.IngredientDto.Carbohydrates * ingredientMeasurementDto.IngredientDto.Carbohydrates;
        }

        return Math.Round(totalCarbohydrates);
    }

    private double GetTotalProtein()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        double totalProtein = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in _recipeDto.IngredientMeasurementDtos)
        {
            totalProtein += ingredientMeasurementDto.IngredientDto.Protein * ingredientMeasurementDto.IngredientDto.Protein;
        }

        return Math.Round(totalProtein);
    }

    private double GetTotalCalories()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        double totalCalories = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in _recipeDto.IngredientMeasurementDtos)
        {
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.IngredientDto.Calories;
        }

        return Math.Round(totalCalories);
    }

    private double GetGramsPerPortion()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        return Math.Round(Convert.ToDouble(GetTotalGrams()) / _recipeDto.PortionAmount);
    }

    private double GetProteinPerPortion()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        return Math.Round(Convert.ToDouble(GetTotalProtein()) / _recipeDto.PortionAmount);
    }

    private double GetCaloriesPerPortion()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        return Math.Round(Convert.ToDouble(GetTotalCalories()) / _recipeDto.PortionAmount);
    }

    private double GetProteinPerCalorie()
    {
        return Math.Round(GetTotalProtein() / GetTotalCalories());
    }
}
