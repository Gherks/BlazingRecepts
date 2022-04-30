using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Shared.Extensions;

public static class RecipeDtoExtensions
{
    public static double GetTotalGrams(this RecipeDto recipeDto)
    {
        double grams = recipeDto.IngredientMeasurementDtos.Sum(ingredientMeasurementDto => ingredientMeasurementDto.Grams);

        return Math.Round(grams, 2);
    }

    public static double GetTotalFat(this RecipeDto recipeDto)
    {
        double totalFat = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalFat += ingredientMeasurementDto.IngredientDto.Fat * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalFat, 2);
    }

    public static double GetTotalCarbohydrates(this RecipeDto recipeDto)
    {
        double totalCarbohydrates = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalCarbohydrates += ingredientMeasurementDto.IngredientDto.Carbohydrates * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalCarbohydrates, 2);
    }

    public static double GetTotalProtein(this RecipeDto recipeDto)
    {
        double totalProtein = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalProtein += ingredientMeasurementDto.IngredientDto.Protein * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalProtein, 2);
    }

    public static double GetTotalCalories(this RecipeDto recipeDto)
    {
        double totalCalories = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.Grams * 0.01f;
        }

        return Math.Round(totalCalories, 2);
    }

    public static double GetGramsPerPortion(this RecipeDto recipeDto)
    {
        double gramsPerPortion = Math.Round(Convert.ToDouble(recipeDto.GetTotalGrams()) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(gramsPerPortion))
        {
            return 0.0;
        }

        return gramsPerPortion;
    }

    public static double GetFatPerPortion(this RecipeDto recipeDto)
    {
        double fatPerPortion = Math.Round(Convert.ToDouble(recipeDto.GetTotalFat()) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(fatPerPortion))
        {
            return 0.0;
        }

        return fatPerPortion;
    }

    public static double GetCarbohydratesPerPortion(this RecipeDto recipeDto)
    {
        double carbohydratesPerPortion = Math.Round(Convert.ToDouble(recipeDto.GetTotalCarbohydrates()) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(carbohydratesPerPortion))
        {
            return 0.0;
        }

        return carbohydratesPerPortion;
    }

    public static double GetProteinPerPortion(this RecipeDto recipeDto)
    {
        double proteinPerPortion = Math.Round(Convert.ToDouble(recipeDto.GetTotalProtein()) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(proteinPerPortion))
        {
            return 0.0;
        }

        return proteinPerPortion;
    }

    public static double GetCaloriesPerPortion(this RecipeDto recipeDto)
    {
        double calroriesPerPortion = Math.Round(Convert.ToDouble(recipeDto.GetTotalCalories()) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(calroriesPerPortion))
        {
            return 0.0;
        }

        return calroriesPerPortion;
    }

    public static double GetProteinPerCalorie(this RecipeDto recipeDto)
    {
        double proteinPerCalorie = Math.Round(recipeDto.GetTotalProtein() / recipeDto.GetTotalCalories(), 2);

        if (double.IsNaN(proteinPerCalorie))
        {
            return 0.0;
        }

        return proteinPerCalorie;
    }
}
