using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Extensions;

public static class RecipeExtensions
{
    public static double GetTotalGrams(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        double grams = recipeDto.IngredientMeasurementDtos.Sum(ingredientMeasurementDto => ingredientMeasurementDto.Grams);

        return Math.Round(grams, 2);
    }

    public static double GetTotalFat(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        double totalFat = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalFat += ingredientMeasurementDto.IngredientDto.Fat * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalFat, 2);
    }

    public static double GetTotalCarbohydrates(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        double totalCarbohydrates = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalCarbohydrates += ingredientMeasurementDto.IngredientDto.Carbohydrates * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalCarbohydrates, 2);
    }

    public static double GetTotalProtein(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        double totalProtein = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalProtein += ingredientMeasurementDto.IngredientDto.Protein * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalProtein, 2);
    }

    public static double GetTotalCalories(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        double totalCalories = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalCalories, 2);
    }

    public static double GetGramsPerPortion(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        return Math.Round(Convert.ToDouble(recipeDto.GetTotalGrams()) / recipeDto.PortionAmount, 2);
    }

    public static double GetProteinPerPortion(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        return Math.Round(Convert.ToDouble(recipeDto.GetTotalProtein()) / recipeDto.PortionAmount, 2);
    }

    public static double GetCaloriesPerPortion(this RecipeDto recipeDto)
    {
        if (recipeDto == null) throw new InvalidOperationException();

        return Math.Round(Convert.ToDouble(recipeDto.GetTotalCalories()) / recipeDto.PortionAmount, 2);
    }

    public static double GetProteinPerCalorie(this RecipeDto recipeDto)
    {
        return Math.Round(recipeDto.GetTotalProtein() / recipeDto.GetTotalCalories(), 2);
    }
}
