using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Contract;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipesPage;

public partial class RecipeCards : PageComponentBase
{
    [Parameter]
    public IEnumerable<RecipeDto>? RecipeDtos { get; set; }

    [Inject]
    protected internal NavigationManager? NavigationManager { get; private set; }

    private void HandleRecipeNavigation(RecipeDto recipeDto)
    {
        Contracts.LogAndThrowWhenNull(NavigationManager, "Cannot navigate to recipe page because navigation manager has not been set.");
        Contracts.LogAndThrowWhenNull(recipeDto, "Cannot navigate to recipe page because recipe is null.");

        NavigationManager.NavigateTo($"recipe/{recipeDto.Id}");
    }

    private int GetProteinPerCalorieStars(RecipeDto recipe)
    {
        Contracts.LogAndThrowWhenNull(recipe, "Cannot calculate protein per calorie stars because recipe is null.");

        double totalProtein = 0.0;
        double totalCalories = 0.0;
        const double PERCENTAGE_TO_DECIMAL = 0.01;

        foreach (var ingredientMeasurementDto in recipe.IngredientMeasurementDtos)
        {
            totalProtein += ingredientMeasurementDto.IngredientDto.Protein * ingredientMeasurementDto.Grams * PERCENTAGE_TO_DECIMAL;
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.Grams * PERCENTAGE_TO_DECIMAL;
        }

        if (totalCalories == 0) return 1;

        double proteinPerCalorie = totalProtein / totalCalories;

        // Calculate star rating based on protein to calorie ratio
        // Higher ratio = more protein per calorie = better
        if (proteinPerCalorie >= 0.12) return 5;
        if (proteinPerCalorie >= 0.09) return 4;
        if (proteinPerCalorie >= 0.06) return 3;
        if (proteinPerCalorie >= 0.03) return 2;
        return 1;
    }
}
