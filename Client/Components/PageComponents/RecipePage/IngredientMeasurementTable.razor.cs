using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Contract;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipePage;

public partial class IngredientMeasurementTable : PageComponentBase
{
    private const double PERCENTAGE_TO_DECIMAL = 0.01;
    private List<CheckableIngredientMeasurement> _checkableIngredientMeasurements = new();

    [CascadingParameter]
    protected internal Recipe? RecipePage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot ingredient measurement table rows because recipe page reference has not been set.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot ingredient measurement table rows because recipe has not been set.");

        foreach (IngredientMeasurementDto ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
        {
            _checkableIngredientMeasurements.Add(new()
            {
                IsChecked = false,
                IngredientMeasurementDto = ingredientMeasurementDto
            });
        }

        IsLoading = false;
    }

    private void HandleRowClick(CheckableIngredientMeasurement checkableIngredientMeasurement)
    {
        checkableIngredientMeasurement.IsChecked = !checkableIngredientMeasurement.IsChecked;
        StateHasChanged();
    }

    private string GetMeasurement(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(ingredientMeasurementDto, "Cannot access ingredient measurement within recipe because passed ingredient measurement has not been set.");
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get measurement because recipe page reference is null.");

        double scaledMeasurement = Math.Round(ingredientMeasurementDto.Measurement * RecipePage.PortionScalingFactor, 2);
        return scaledMeasurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }

    private string GetIngredientMeasurementRowClass(CheckableIngredientMeasurement checkableIngredientMeasurement)
    {
        Contracts.LogAndThrowWhenNull(checkableIngredientMeasurement, "Cannot set class on ingredient measurement table row because checkable ingredient measurement has not been set.");

        return checkableIngredientMeasurement.IsChecked ? "table-primary" : "";
    }

    private double GetScaledGrams(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled grams because recipe page reference is null.");
        return Math.Round(ingredientMeasurementDto.Grams * RecipePage.PortionScalingFactor, 2);
    }

    private double GetScaledFat(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled fat because recipe page reference is null.");
        double scaledGrams = ingredientMeasurementDto.Grams * RecipePage.PortionScalingFactor;
        double fat = ingredientMeasurementDto.IngredientDto.Fat * scaledGrams * PERCENTAGE_TO_DECIMAL;
        return Math.Round(fat, 2);
    }

    private double GetScaledCarbohydrates(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled carbohydrates because recipe page reference is null.");
        double scaledGrams = ingredientMeasurementDto.Grams * RecipePage.PortionScalingFactor;
        double carbohydrates = ingredientMeasurementDto.IngredientDto.Carbohydrates * scaledGrams * PERCENTAGE_TO_DECIMAL;
        return Math.Round(carbohydrates, 2);
    }

    private double GetScaledProtein(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled protein because recipe page reference is null.");
        double scaledGrams = ingredientMeasurementDto.Grams * RecipePage.PortionScalingFactor;
        double protein = ingredientMeasurementDto.IngredientDto.Protein * scaledGrams * PERCENTAGE_TO_DECIMAL;
        return Math.Round(protein, 2);
    }

    private double GetScaledCalories(IngredientMeasurementDto ingredientMeasurementDto)
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled calories because recipe page reference is null.");
        double scaledGrams = ingredientMeasurementDto.Grams * RecipePage.PortionScalingFactor;
        double calories = ingredientMeasurementDto.IngredientDto.Calories * scaledGrams * PERCENTAGE_TO_DECIMAL;
        return Math.Round(calories, 2);
    }

    private double GetScaledTotalGrams()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled total grams because recipe page reference is null.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot get scaled total grams because recipe has not been set.");
        double totalGrams = RecipePage.RecipeDto.IngredientMeasurementDtos.Sum(i => i.Grams);
        return Math.Round(totalGrams * RecipePage.PortionScalingFactor, 2);
    }

    private double GetScaledTotalFat()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled total fat because recipe page reference is null.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot get scaled total fat because recipe has not been set.");
        double totalFat = 0.0;
        foreach (IngredientMeasurementDto ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
        {
            totalFat += ingredientMeasurementDto.IngredientDto.Fat * ingredientMeasurementDto.Grams * PERCENTAGE_TO_DECIMAL;
        }
        return Math.Round(totalFat * RecipePage.PortionScalingFactor, 2);
    }

    private double GetScaledTotalCarbohydrates()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled total carbohydrates because recipe page reference is null.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot get scaled total carbohydrates because recipe has not been set.");
        double totalCarbohydrates = 0.0;
        foreach (IngredientMeasurementDto ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
        {
            totalCarbohydrates += ingredientMeasurementDto.IngredientDto.Carbohydrates * ingredientMeasurementDto.Grams * PERCENTAGE_TO_DECIMAL;
        }
        return Math.Round(totalCarbohydrates * RecipePage.PortionScalingFactor, 2);
    }

    private double GetScaledTotalProtein()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled total protein because recipe page reference is null.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot get scaled total protein because recipe has not been set.");
        double totalProtein = 0.0;
        foreach (IngredientMeasurementDto ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
        {
            totalProtein += ingredientMeasurementDto.IngredientDto.Protein * ingredientMeasurementDto.Grams * PERCENTAGE_TO_DECIMAL;
        }
        return Math.Round(totalProtein * RecipePage.PortionScalingFactor, 2);
    }

    private double GetScaledTotalCalories()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled total calories because recipe page reference is null.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot get scaled total calories because recipe has not been set.");
        double totalCalories = 0.0;
        foreach (IngredientMeasurementDto ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
        {
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.Grams * PERCENTAGE_TO_DECIMAL;
        }
        return Math.Round(totalCalories * RecipePage.PortionScalingFactor, 2);
    }

    private class CheckableIngredientMeasurement
    {
        public bool IsChecked { get; set; } = false;
        public IngredientMeasurementDto IngredientMeasurementDto { get; set; } = new();
    }
}
