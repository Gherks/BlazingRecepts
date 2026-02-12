using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Contract;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipePage;

public partial class RecipeNutritionCards : PageComponentBase
{
    private const double PERCENTAGE_TO_DECIMAL = 0.01;

    [CascadingParameter]
    protected internal Recipe? RecipePage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot load nutrition cards because recipe page reference has not been set.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot load nutrition cards because recipe has not been set.");

        IsLoading = false;
    }

    private double GetScaledTotalProtein()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get scaled total protein because recipe page reference is null.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot get scaled total protein because recipe has not been set.");
        double totalProtein = 0.0;
        foreach (var ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
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
        foreach (var ingredientMeasurementDto in RecipePage.RecipeDto.IngredientMeasurementDtos)
        {
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.Grams * PERCENTAGE_TO_DECIMAL;
        }
        return Math.Round(totalCalories * RecipePage.PortionScalingFactor, 2);
    }

    private double GetProteinPerPortion()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get protein per portion because recipe page reference is null.");
        double proteinPerPortion = Math.Round(GetScaledTotalProtein() / RecipePage.CurrentPortionAmount, 2);
        return double.IsNaN(proteinPerPortion) ? 0.0 : proteinPerPortion;
    }

    private double GetCaloriesPerPortion()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot get calories per portion because recipe page reference is null.");
        double caloriesPerPortion = Math.Round(GetScaledTotalCalories() / RecipePage.CurrentPortionAmount, 2);
        return double.IsNaN(caloriesPerPortion) ? 0.0 : caloriesPerPortion;
    }

    private double GetProteinPerCalorie()
    {
        double totalCalories = GetScaledTotalCalories();
        if (totalCalories == 0) return 0.0;
        double proteinPerCalorie = Math.Round(GetScaledTotalProtein() / totalCalories, 2);
        return double.IsNaN(proteinPerCalorie) ? 0.0 : proteinPerCalorie;
    }

    private int GetProteinPerCalorieStars()
    {
        double proteinPerCalorie = GetProteinPerCalorie();
        
        // Calculate star rating based on protein to calorie ratio
        // Higher ratio = more protein per calorie = better
        // Typical ranges:
        // < 0.05 = 1 star (low protein)
        // 0.05 - 0.10 = 2 stars (below average)
        // 0.10 - 0.15 = 3 stars (average)
        // 0.15 - 0.20 = 4 stars (good)
        // >= 0.20 = 5 stars (excellent)
        
        if (proteinPerCalorie >= 0.20) return 5;
        if (proteinPerCalorie >= 0.15) return 4;
        if (proteinPerCalorie >= 0.10) return 3;
        if (proteinPerCalorie >= 0.05) return 2;
        return 1;
    }
}
