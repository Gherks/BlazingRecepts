using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using BlazingRecept.Shared.Extensions;
using Serilog;

namespace BlazingRecept.Server.Services;

public class DailyIntakeEntryService : IDailyIntakeEntryService
{
    private const string _logProperty = "Domain";

    private readonly IDailyIntakeEntryRepository _dailyIntakeEntryRepository;
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;
    private readonly IMapper _mapper;

    public DailyIntakeEntryService(
        IDailyIntakeEntryRepository dailyIntakeEntryRepository,
        IRecipeService recipeService,
        IIngredientService ingredientService,
        IMapper mapper)
    {
        _dailyIntakeEntryRepository = dailyIntakeEntryRepository;
        _recipeService = recipeService;
        _ingredientService = ingredientService;
        _mapper = mapper;
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _dailyIntakeEntryRepository.AnyAsync(id);
    }

    public async Task<DailyIntakeEntryDto?> GetByIdAsync(Guid id)
    {
        DailyIntakeEntry? dailyIntakeEntry = await _dailyIntakeEntryRepository.GetByIdAsync(id);

        if (dailyIntakeEntry != null)
        {
            return await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);
        }

        return null;
    }

    public async Task<IReadOnlyList<DailyIntakeEntryDto>?> GetAllAsync()
    {
        IReadOnlyList<DailyIntakeEntry>? dailyIntakeEntries = await _dailyIntakeEntryRepository.ListAllAsync();

        Contracts.LogAndThrowWhenNull(dailyIntakeEntries, "Failed to fetch all daily intake entries from repository.");

        List<DailyIntakeEntryDto> dailyIntakeEntryDtos = new();

        foreach (DailyIntakeEntry dailyIntakeEntry in dailyIntakeEntries)
        {
            DailyIntakeEntryDto? dailyIntakeEntryDto = await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);

            Contracts.LogAndThrowWhenNull(dailyIntakeEntryDto, "Failed while loading one of many daily intake entries into a daily intake entry dto.");

            dailyIntakeEntryDtos.Add(dailyIntakeEntryDto);
        }

        return dailyIntakeEntryDtos;
    }

    public async Task<DailyIntakeEntryDto> SaveAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        DailyIntakeEntry? dailyIntakeEntry = _mapper.Map<DailyIntakeEntry>(dailyIntakeEntryDto);

        dailyIntakeEntry.ProductId = await GetProductIdFromProductName(dailyIntakeEntryDto.ProductName);

        if (dailyIntakeEntry.Id == Guid.Empty)
        {
            dailyIntakeEntry = await _dailyIntakeEntryRepository.AddAsync(dailyIntakeEntry);
        }
        else
        {
            dailyIntakeEntry = await _dailyIntakeEntryRepository.UpdateAsync(dailyIntakeEntry);
        }

        Contracts.LogAndThrowWhenNull(dailyIntakeEntry, $"Failed to add or update daily intake entry({dailyIntakeEntryDto}), result was null.");

        DailyIntakeEntryDto? reloadedDailyIntakeEntryDto = await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);

        Contracts.LogAndThrowWhenNull(reloadedDailyIntakeEntryDto, $"Failed to reload added or updated daily intake entry({dailyIntakeEntryDto}), result was null.");

        return reloadedDailyIntakeEntryDto;
    }

    public async Task<bool> SaveAsync(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        foreach (DailyIntakeEntryDto dailyIntakeEntryDto in dailyIntakeEntryDtos)
        {
            DailyIntakeEntry dailyIntakeEntry = _mapper.Map<DailyIntakeEntry>(dailyIntakeEntryDto);

            dailyIntakeEntry.ProductId = await GetProductIdFromProductName(dailyIntakeEntryDto.ProductName);

            try
            {
                if (dailyIntakeEntry.Id == Guid.Empty)
                {
                    await _dailyIntakeEntryRepository.AddAsync(dailyIntakeEntry);
                }
                else
                {
                    await _dailyIntakeEntryRepository.UpdateAsync(dailyIntakeEntry);
                }
            }
            catch (Exception exception)
            {
                string messageTemplate = $"Something went wrong while saving and/or updating multiple daily intake entries: {dailyIntakeEntryDtos}";
                Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, dailyIntakeEntryDtos);
            }
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        DailyIntakeEntry? dailyIntakeEntry = await _dailyIntakeEntryRepository.GetByIdAsync(id);

        if (dailyIntakeEntry != null)
        {
            return await _dailyIntakeEntryRepository.DeleteAsync(dailyIntakeEntry);
        }

        return false;
    }

    private async Task<Guid> GetProductIdFromProductName(string productName)
    {
        RecipeDto? recipeDto = await _recipeService.GetByNameAsync(productName);

        if (recipeDto != null)
        {
            return recipeDto.Id;
        }
        else
        {
            IngredientDto? ingredientDto = await _ingredientService.GetByNameAsync(productName);

            if (ingredientDto != null)
            {
                return ingredientDto.Id;
            }
        }

        const string errorMessage = "Could not find product id from given product name because product with given name was not found in neither recipes nor ingredients.";
        Log.ForContext(_logProperty, GetType().Name).Error(errorMessage);
        throw new InvalidOperationException(errorMessage);
    }

    private async Task<DailyIntakeEntryDto?> LoadDailyIntakeEntryDtoFromDailyIntakeEntry(DailyIntakeEntry dailyIntakeEntry)
    {
        DailyIntakeEntryDto dailyIntakeEntryDto = _mapper.Map<DailyIntakeEntryDto>(dailyIntakeEntry);

        RecipeDto? recipeDto = await _recipeService.GetByIdAsync(dailyIntakeEntryDto.ProductId);

        if (recipeDto != null)
        {
            AddRecipeDataIntoDailyIntakeEntry(dailyIntakeEntryDto, recipeDto);
        }
        else
        {
            IngredientDto? ingredientDto = await _ingredientService.GetByIdAsync(dailyIntakeEntryDto.ProductId);

            Contracts.LogAndThrowWhenNull(ingredientDto, "Cannot load daily intake entry by id from services because its product was not found in neither recipes nor ingredients.");

            AddIngredientDataIntoDailyIntakeEntry(dailyIntakeEntryDto, ingredientDto);
        }

        return dailyIntakeEntryDto;
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
