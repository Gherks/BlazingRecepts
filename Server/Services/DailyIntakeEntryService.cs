using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using BlazingRecept.Shared.Extensions;
using Serilog;

namespace BlazingRecept.Server.Services
{
    public class DailyIntakeEntryService : IDailyIntakeEntryService
    {
        private const string _logProperty = "Domain";
        private const string _logDomainName = "DailyIntakeEntryService";

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

            if (dailyIntakeEntries == null)
            {
                const string errorMessage = "Failed to fetch all daily intake entries and turn them into daily intake entry dtos.";
                Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            List<DailyIntakeEntryDto> dailyIntakeEntryDtos = new();

            foreach (DailyIntakeEntry dailyIntakeEntry in dailyIntakeEntries)
            {
                DailyIntakeEntryDto? dailyIntakeEntryDto = await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);

                if (dailyIntakeEntryDto == null)
                {
                    const string errorMessage = "Failed while loading one of many daily intake entries into a daily intake entry dto.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                dailyIntakeEntryDtos.Add(dailyIntakeEntryDto);
            }

            return dailyIntakeEntryDtos;
        }

        public async Task<DailyIntakeEntryDto> SaveAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
        {
            DailyIntakeEntry dailyIntakeEntry = _mapper.Map<DailyIntakeEntry>(dailyIntakeEntryDto);

            if (dailyIntakeEntry.Id == Guid.Empty)
            {
                dailyIntakeEntry = await _dailyIntakeEntryRepository.AddAsync(dailyIntakeEntry);
            }
            else
            {
                dailyIntakeEntry = await _dailyIntakeEntryRepository.UpdateAsync(dailyIntakeEntry);
            }

            DailyIntakeEntryDto? reloadedDailyIntakeEntryDto = await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);

            if (reloadedDailyIntakeEntryDto == null)
            {
                const string messageTemplate = "Failed to reload added or updated daily intake entry({@DailyIntakeEntryDto}), result was null.";
                Log.ForContext(_logProperty, _logDomainName).Error(messageTemplate, dailyIntakeEntryDto);
                throw new InvalidOperationException("Failed to reload added or updated daily intake entry, result was null.");
            }

            return reloadedDailyIntakeEntryDto;
        }

        public async Task<bool> SaveAsync(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
        {
            foreach (DailyIntakeEntryDto dailyIntakeEntryDto in dailyIntakeEntryDtos)
            {
                DailyIntakeEntry dailyIntakeEntry = _mapper.Map<DailyIntakeEntry>(dailyIntakeEntryDto);

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
                    const string errorMessage = "Something went wrong while saving and/or updating multiple daily intake entries: ({@DailyIntakeEntries})";
                    Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, dailyIntakeEntryDtos);
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

                if (ingredientDto != null)
                {
                    AddIngredientDataIntoDailyIntakeEntry(dailyIntakeEntryDto, ingredientDto);
                }
                else
                {
                    const string errorMessage = "Cannot load daily intake entry by id from services because its product was not found in neither recipes nor ingredients.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
            }

            return dailyIntakeEntryDto;
        }

        private void AddIngredientDataIntoDailyIntakeEntry(DailyIntakeEntryDto dailyIntakeEntryDto, IngredientDto ingredientDto)
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

        private void AddRecipeDataIntoDailyIntakeEntry(DailyIntakeEntryDto dailyIntakeEntryDto, RecipeDto recipeDto)
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
}
