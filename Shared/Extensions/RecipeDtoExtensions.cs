using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Shared.Extensions;

public static class RecipeDtoExtensions
{
    // Helper method to calculate all totals in a single pass
    private record struct RecipeTotals(double Grams, double Fat, double Carbohydrates, double Protein, double Calories);

    private static RecipeTotals CalculateTotals(RecipeDto recipeDto)
    {
        double totalGrams = 0.0;
        double totalFat = 0.0;
        double totalCarbohydrates = 0.0;
        double totalProtein = 0.0;
        double totalCalories = 0.0;

        foreach (IngredientMeasurementDto ingredientMeasurementDto in recipeDto.IngredientMeasurementDtos)
        {
            double grams = ingredientMeasurementDto.Grams;
            double gramsMultiplier = grams * 0.01;
            
            totalGrams += grams;
            totalFat += ingredientMeasurementDto.IngredientDto.Fat * gramsMultiplier;
            totalCarbohydrates += ingredientMeasurementDto.IngredientDto.Carbohydrates * gramsMultiplier;
            totalProtein += ingredientMeasurementDto.IngredientDto.Protein * gramsMultiplier;
            totalCalories += ingredientMeasurementDto.IngredientDto.Calories * gramsMultiplier;
        }

        return new RecipeTotals(
            Math.Round(totalGrams, 2),
            Math.Round(totalFat, 2),
            Math.Round(totalCarbohydrates, 2),
            Math.Round(totalProtein, 2),
            Math.Round(totalCalories, 2)
        );
    }

    public static double GetTotalGrams(this RecipeDto recipeDto)
    {
        return CalculateTotals(recipeDto).Grams;
    }

    public static double GetTotalFat(this RecipeDto recipeDto)
    {
        return CalculateTotals(recipeDto).Fat;
    }

    public static double GetTotalCarbohydrates(this RecipeDto recipeDto)
    {
        return CalculateTotals(recipeDto).Carbohydrates;
    }

    public static double GetTotalProtein(this RecipeDto recipeDto)
    {
        return CalculateTotals(recipeDto).Protein;
    }

    public static double GetTotalCalories(this RecipeDto recipeDto)
    {
        return CalculateTotals(recipeDto).Calories;
    }

    public static double GetGramsPerPortion(this RecipeDto recipeDto)
    {
        double gramsPerPortion = Math.Round(Convert.ToDouble(CalculateTotals(recipeDto).Grams) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(gramsPerPortion))
        {
            return 0.0;
        }

        return gramsPerPortion;
    }

    public static double GetFatPerPortion(this RecipeDto recipeDto)
    {
        double fatPerPortion = Math.Round(Convert.ToDouble(CalculateTotals(recipeDto).Fat) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(fatPerPortion))
        {
            return 0.0;
        }

        return fatPerPortion;
    }

    public static double GetCarbohydratesPerPortion(this RecipeDto recipeDto)
    {
        double carbohydratesPerPortion = Math.Round(Convert.ToDouble(CalculateTotals(recipeDto).Carbohydrates) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(carbohydratesPerPortion))
        {
            return 0.0;
        }

        return carbohydratesPerPortion;
    }

    public static double GetProteinPerPortion(this RecipeDto recipeDto)
    {
        double proteinPerPortion = Math.Round(Convert.ToDouble(CalculateTotals(recipeDto).Protein) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(proteinPerPortion))
        {
            return 0.0;
        }

        return proteinPerPortion;
    }

    public static double GetCaloriesPerPortion(this RecipeDto recipeDto)
    {
        double calroriesPerPortion = Math.Round(Convert.ToDouble(CalculateTotals(recipeDto).Calories) / recipeDto.PortionAmount, 2);

        if (double.IsNaN(calroriesPerPortion))
        {
            return 0.0;
        }

        return calroriesPerPortion;
    }

    public static double GetProteinPerCalorie(this RecipeDto recipeDto)
    {
        var totals = CalculateTotals(recipeDto);
        double proteinPerCalorie = Math.Round(totals.Protein / totals.Calories, 2);

        if (double.IsNaN(proteinPerCalorie))
        {
            return 0.0;
        }

        return proteinPerCalorie;
    }
}
