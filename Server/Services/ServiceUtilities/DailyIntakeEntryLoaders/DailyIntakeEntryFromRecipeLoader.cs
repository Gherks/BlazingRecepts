using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using BlazingRecept.Shared.Extensions;

namespace BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaders;

public sealed class DailyIntakeEntryFromRecipeLoader : IDailyIntakeEntryLoader
{
    private readonly IRecipeService _recipeService;

    public DailyIntakeEntryFromRecipeLoader(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<Guid?> GetProductIdFromProductName(string productName)
    {
        RecipeDto? recipeDto = await _recipeService.GetByNameAsync(productName);

        if (recipeDto != null)
        {
            return recipeDto.Id;
        }

        return null;
    }

    public async Task<DailyIntakeEntryDto?> LoadProductDataForDailyIntakeEntryDto(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        RecipeDto? recipeDto = await _recipeService.GetByIdAsync(dailyIntakeEntryDto.ProductId);

        if (recipeDto != null)
        {
            AddRecipeDataIntoDailyIntakeEntry(dailyIntakeEntryDto, recipeDto);

            return dailyIntakeEntryDto;
        }

        return null;
    }

    private void AddRecipeDataIntoDailyIntakeEntry(DailyIntakeEntryDto dailyIntakeEntryDto, RecipeDto recipeDto)
    {
        double portionMultiplier = dailyIntakeEntryDto.Amount;

        dailyIntakeEntryDto.ProductName = recipeDto.Name;
        dailyIntakeEntryDto.Fat = recipeDto.GetFatPerPortion() * portionMultiplier;
        dailyIntakeEntryDto.Carbohydrates = recipeDto.GetCarbohydratesPerPortion() * portionMultiplier;
        dailyIntakeEntryDto.Protein = recipeDto.GetProteinPerPortion() * portionMultiplier;
        dailyIntakeEntryDto.Calories = recipeDto.GetCaloriesPerPortion() * portionMultiplier;
        dailyIntakeEntryDto.ProteinPerCalorie = recipeDto.GetProteinPerCalorie();
        dailyIntakeEntryDto.IsRecipe = true;
        dailyIntakeEntryDto.ProductId = recipeDto.Id;
    }
}
