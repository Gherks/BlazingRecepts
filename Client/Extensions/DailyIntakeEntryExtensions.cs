using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Serilog;

namespace BlazingRecept.Client.Extensions;

public static class DailyIntakeEntryExtensions
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "DailyIntakeEntryExtensions";

    public static async Task LoadFromProductServices(
        this DailyIntakeEntryDto dailyIntakeEntryDto,
        IIngredientService ingredientService,
        IRecipeService recipeService)
    {
        RecipeDto? recipeDto = await recipeService.GetByIdAsync(dailyIntakeEntryDto.ProductId);

        if (recipeDto != null)
        {
            AddRecipeDataIntoDailyIntakeEntry(dailyIntakeEntryDto, recipeDto);
        }
        else
        {
            IngredientDto? ingredientDto = await ingredientService.GetByIdAsync(dailyIntakeEntryDto.ProductId);

            if (ingredientDto != null)
            {
                AddIngredientDataIntoDailyIntakeEntry(dailyIntakeEntryDto, ingredientDto);
            }
            else
            {
                const string errorMessage = "Cannot load daily intake entry by id from services because its product was not found in either recipes or ingredients.";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }

    public static void LoadFromProductListsById(
        this DailyIntakeEntryDto dailyIntakeEntryDto,
        IReadOnlyList<IngredientDto> ingredientDtos,
        IReadOnlyList<RecipeDto> recipeDtos)
    {
        RecipeDto? recipeDto = recipeDtos.FirstOrDefault(recipeDto => recipeDto.Id == dailyIntakeEntryDto.ProductId);

        if (recipeDto != null)
        {
            AddRecipeDataIntoDailyIntakeEntry(dailyIntakeEntryDto, recipeDto);
        }
        else
        {
            IngredientDto? ingredientDto = ingredientDtos.FirstOrDefault(ingredientDto => ingredientDto.Id == dailyIntakeEntryDto.ProductId);

            if (ingredientDto != null)
            {
                AddIngredientDataIntoDailyIntakeEntry(dailyIntakeEntryDto, ingredientDto);
            }
            else
            {
                const string errorMessage = "Cannot load daily intake by id from product lists because its was not found in either list of recipes or ingredients.";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }

    public static void LoadFromProductListsByName(
        this DailyIntakeEntryDto dailyIntakeEntryDto,
        IReadOnlyList<IngredientDto> ingredientDtos,
        IReadOnlyList<RecipeDto> recipeDtos)
    {
        RecipeDto? recipeDto = recipeDtos.FirstOrDefault(recipeDto => recipeDto.Name == dailyIntakeEntryDto.ProductName);

        if (recipeDto != null)
        {
            AddRecipeDataIntoDailyIntakeEntry(dailyIntakeEntryDto, recipeDto);
        }
        else
        {
            IngredientDto? ingredientDto = ingredientDtos.FirstOrDefault(ingredientDto => ingredientDto.Name == dailyIntakeEntryDto.ProductName);

            if (ingredientDto != null)
            {
                AddIngredientDataIntoDailyIntakeEntry(dailyIntakeEntryDto, ingredientDto);
            }
            else
            {
                const string errorMessage = "Cannot load daily intake entry by name from product lists because its was not found in either list of recipes or ingredients.";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }

    private static void AddIngredientDataIntoDailyIntakeEntry(DailyIntakeEntryDto dailyIntakeEntryDto, IngredientDto ingredientDto)
    {
        double gramMultiplier = dailyIntakeEntryDto.Amount * 0.01;

        dailyIntakeEntryDto.ProductName = ingredientDto.Name;
        dailyIntakeEntryDto.Fat = ingredientDto.Fat * gramMultiplier;
        dailyIntakeEntryDto.Carbohydrates = ingredientDto.Carbohydrates * gramMultiplier;
        dailyIntakeEntryDto.Protein = ingredientDto.Protein * gramMultiplier;
        dailyIntakeEntryDto.Calories = ingredientDto.Calories * gramMultiplier;
        dailyIntakeEntryDto.ProteinPerCalorie = Math.Round(ingredientDto.Protein / ingredientDto.Calories, 2);
        dailyIntakeEntryDto.IsRecipe = false;
        dailyIntakeEntryDto.ProductId = ingredientDto.Id;
    }

    private static void AddRecipeDataIntoDailyIntakeEntry(DailyIntakeEntryDto dailyIntakeEntryDto, RecipeDto recipeDto)
    {
        double portionMultiplier = dailyIntakeEntryDto.Amount;

        dailyIntakeEntryDto.ProductName = recipeDto.Name;
        dailyIntakeEntryDto.Fat = recipeDto.GetTotalFat() * portionMultiplier;
        dailyIntakeEntryDto.Carbohydrates = recipeDto.GetTotalCarbohydrates() * portionMultiplier;
        dailyIntakeEntryDto.Protein = recipeDto.GetTotalProtein() * portionMultiplier;
        dailyIntakeEntryDto.Calories = recipeDto.GetTotalCalories() * portionMultiplier;
        dailyIntakeEntryDto.ProteinPerCalorie = recipeDto.GetProteinPerCalorie();
        dailyIntakeEntryDto.IsRecipe = true;
        dailyIntakeEntryDto.ProductId = recipeDto.Id;
    }
}
