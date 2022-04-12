using BlazingRecept.Shared.Dto;
using Serilog;

namespace BlazingRecept.Client.Extensions;

public static class RecipeExtensions
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipeExtensions";

    public static double GetTotalGrams(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe total grams because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        double grams = recipeDto.IngredientMeasurementDtos.Sum(ingredientMeasurementDto => ingredientMeasurementDto.Grams);

        return Math.Round(grams, 2);
    }

    public static double GetTotalFat(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe total fat because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        double totalFat = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalFat += ingredientMeasurementDto.IngredientDto.Fat * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalFat, 2);
    }

    public static double GetTotalCarbohydrates(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe total carbohydrates because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        double totalCarbohydrates = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalCarbohydrates += ingredientMeasurementDto.IngredientDto.Carbohydrates * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalCarbohydrates, 2);
    }

    public static double GetTotalProtein(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe total protein because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        double totalProtein = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalProtein += ingredientMeasurementDto.IngredientDto.Protein * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalProtein, 2);
    }

    public static double GetTotalCalories(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe total calories because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        double totalCalories = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalCalories, 2);
    }

    public static double GetGramsPerPortion(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe grams per portion because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        return Math.Round(Convert.ToDouble(recipeDto.GetTotalGrams()) / recipeDto.PortionAmount, 2);
    }

    public static double GetProteinPerPortion(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe protein per portion because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        return Math.Round(Convert.ToDouble(recipeDto.GetTotalProtein()) / recipeDto.PortionAmount, 2);
    }

    public static double GetCaloriesPerPortion(this RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            string errorMessage = "Cannot access recipe calories per portion because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(recipeDto), errorMessage);
        }

        return Math.Round(Convert.ToDouble(recipeDto.GetTotalCalories()) / recipeDto.PortionAmount, 2);
    }

    public static double GetProteinPerCalorie(this RecipeDto recipeDto)
    {
        return Math.Round(recipeDto.GetTotalProtein() / recipeDto.GetTotalCalories(), 2);
    }
}
