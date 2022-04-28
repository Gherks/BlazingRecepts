using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Serilog;

namespace BlazingRecept.Server.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientService _ingredientService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public RecipeService(
            IRecipeRepository recipeRepository,
            IIngredientService ingredientService,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _ingredientService = ingredientService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<bool> AnyAsync(Guid id)
        {
            return await _recipeRepository.AnyAsync(id);
        }

        public async Task<bool> AnyAsync(string name)
        {
            return await _recipeRepository.AnyAsync(name);
        }

        public async Task<RecipeDto?> GetByIdAsync(Guid id)
        {
            Recipe? recipe = await _recipeRepository.GetByIdAsync(id);

            if (recipe != null)
            {
                RecipeDto recipeDto = _mapper.Map<RecipeDto>(recipe);
                IReadOnlyList<IngredientDto>? ingredientDtos = await _ingredientService.GetAllAsync();

                if (ingredientDtos == null)
                {
                    const string errorMessage = "Cannot reload saved recipe because fetched ingredient list is null.";
                    Log.Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                LoadIngredientMeasurements(recipeDto, recipe, ingredientDtos);

                return recipeDto;
            }

            return null;
        }

        public async Task<IReadOnlyList<RecipeDto>?> GetAllAsync()
        {
            IReadOnlyList<Recipe>? recipes = await _recipeRepository.ListAllAsync();

            if (recipes == null)
            {
                const string errorMessage = "Failed because fetched recipe list is null.";
                Log.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            //return recipes.Select(recipe => _mapper.Map<RecipeDto>(recipe)).ToList();
            List<RecipeDto> recipeDtos = new();
            IReadOnlyList<IngredientDto>? ingredientDtos = await _ingredientService.GetAllAsync();
            foreach(Recipe recipe in recipes)
            {
                RecipeDto recipeDto = _mapper.Map<RecipeDto>(recipe);

                if (ingredientDtos == null)
                {
                    const string errorMessage = "Cannot fetch all recipe dtos because fetched ingredient list is null.";
                    Log.Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                LoadIngredientMeasurements(recipeDto, recipe, ingredientDtos);
                recipeDtos.Add(recipeDto);
            }

            return recipeDtos;
        }

        public async Task<RecipeDto> SaveAsync(RecipeDto recipeDto)
        {
            Recipe recipe = _mapper.Map<Recipe>(recipeDto);

            if (recipe.Id == Guid.Empty)
            {
                recipe = await _recipeRepository.AddAsync(recipe);
            }
            else
            {
                recipe = await _recipeRepository.UpdateAsync(recipe);
            }

            recipeDto = _mapper.Map<RecipeDto>(recipe);

            IReadOnlyList<IngredientDto>? ingredientDtos = await _ingredientService.GetAllAsync();

            if (ingredientDtos == null)
            {
                const string errorMessage = "Cannot reload saved recipe because fetched ingredient list is null.";
                Log.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            LoadIngredientMeasurements(recipeDto, recipe, ingredientDtos);

            return recipeDto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Recipe? recipe = await _recipeRepository.GetByIdAsync(id);

            if (recipe != null)
            {
                return await _recipeRepository.DeleteAsync(recipe);
            }

            return false;
        }

        private RecipeDto LoadIngredientMeasurements(RecipeDto recipeDto, Recipe recipe, IReadOnlyList<IngredientDto> ingredientDtos)
        {
            recipeDto.IngredientMeasurementDtos.Clear();

            foreach (IngredientMeasurement ingredientMeasurement in recipe.IngredientMeasurements)
            {
                IngredientMeasurementDto ingredientMeasurementDto = _mapper.Map<IngredientMeasurementDto>(ingredientMeasurement);
                ingredientMeasurementDto.IngredientDto = ingredientDtos.First(ingredientDto => ingredientDto.Id == ingredientMeasurement.IngredientId);

                recipeDto.IngredientMeasurementDtos.Add(ingredientMeasurementDto);
            }

            recipeDto.IngredientMeasurementDtos.Sort((first, second) => first.SortOrder > second.SortOrder ? 1 : -1);

            return recipeDto;
        }
    }
}