using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaders;

public sealed class DailyIntakeEntryFromIngredientLoader : IDailyIntakeEntryLoader
{
    private readonly IIngredientService _ingredientService;

    public DailyIntakeEntryFromIngredientLoader(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<Guid?> GetProductIdFromProductName(string productName)
    {
        IngredientDto? ingredientDto = await _ingredientService.GetByNameAsync(productName);

        if (ingredientDto != null)
        {
            return ingredientDto.Id;
        }

        return null;
    }

    public async Task<DailyIntakeEntryDto?> LoadProductDataForDailyIntakeEntryDto(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        IngredientDto? ingredientDto = await _ingredientService.GetByIdAsync(dailyIntakeEntryDto.ProductId);

        if (ingredientDto != null)
        {
            AddIngredientDataIntoDailyIntakeEntry(dailyIntakeEntryDto, ingredientDto);

            return dailyIntakeEntryDto;
        }

        return null;
    }

    private void AddIngredientDataIntoDailyIntakeEntry(DailyIntakeEntryDto dailyIntakeEntryDto, IngredientDto ingredientDto)
    {
        double gramMultiplier = dailyIntakeEntryDto.Amount * 0.01;
        double proteinPerCalorie = Math.Round(ingredientDto.Protein / ingredientDto.Calories, 2);

        if (double.IsNaN(proteinPerCalorie))
        {
            proteinPerCalorie = 0.0;
        }

        dailyIntakeEntryDto.ProductName = ingredientDto.Name;
        dailyIntakeEntryDto.Fat = ingredientDto.Fat * gramMultiplier;
        dailyIntakeEntryDto.Carbohydrates = ingredientDto.Carbohydrates * gramMultiplier;
        dailyIntakeEntryDto.Protein = ingredientDto.Protein * gramMultiplier;
        dailyIntakeEntryDto.Calories = ingredientDto.Calories * gramMultiplier;
        dailyIntakeEntryDto.ProteinPerCalorie = proteinPerCalorie;
        dailyIntakeEntryDto.IsRecipe = false;
        dailyIntakeEntryDto.ProductId = ingredientDto.Id;
    }
}
