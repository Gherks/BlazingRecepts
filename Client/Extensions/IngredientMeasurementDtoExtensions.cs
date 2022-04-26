using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Extensions;

public static class IngredientMeasurementDtoExtensions
{
    public static double GetFat(this IngredientMeasurementDto ingredientMeasurementDto)
    {
        double fat = ingredientMeasurementDto.IngredientDto.Fat * ingredientMeasurementDto.Grams * 0.01f;

        return Math.Round(fat, 2);
    }

    public static double GetCarbohydrates(this IngredientMeasurementDto ingredientMeasurementDto)
    {
        double carbohydrates = ingredientMeasurementDto.IngredientDto.Carbohydrates * ingredientMeasurementDto.Grams * 0.01f;

        return Math.Round(carbohydrates, 2);
    }

    public static double GetProtein(this IngredientMeasurementDto ingredientMeasurementDto)
    {
        double protein = ingredientMeasurementDto.IngredientDto.Protein * ingredientMeasurementDto.Grams * 0.01f;

        return Math.Round(protein, 2);
    }

    public static double GetCalories(this IngredientMeasurementDto ingredientMeasurementDto)
    {
        double calories = ingredientMeasurementDto.IngredientDto.Calories * ingredientMeasurementDto.Grams * 0.01f;

        return Math.Round(calories, 2);
    }
}
