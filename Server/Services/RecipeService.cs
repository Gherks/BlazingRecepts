using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientService _ingredientService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public RecipeService(IRecipeRepository recipeRepository, IIngredientService ingredientService, ICategoryService categoryService, IMapper mapper)
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
                IReadOnlyList<IngredientDto> ingredientDtos = await _ingredientService.GetAllAsync();

                RecipeDto recipeDto = await LoadRecipeDtoFromRecipe(recipe, ingredientDtos);

                return recipeDto;
            }

            return null;
        }

        public async Task<IReadOnlyList<RecipeDto>> GetAllAsync()
        {
            IReadOnlyList<Recipe> recipes = await _recipeRepository.ListAllAsync() ?? new List<Recipe>();

            IReadOnlyList<IngredientDto> ingredientDtos = await _ingredientService.GetAllAsync();

            List<RecipeDto> recipeDto = new();

            foreach (Recipe recipe in recipes)
            {
                recipeDto.Add(await LoadRecipeDtoFromRecipe(recipe, ingredientDtos));
            }

            return recipeDto;
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
                await _recipeRepository.UpdateAsync(recipe);
            }

            IReadOnlyList<IngredientDto> ingredientDtos = await _ingredientService.GetAllAsync();

            return await LoadRecipeDtoFromRecipe(recipe, ingredientDtos);
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

        private async Task<RecipeDto> LoadRecipeDtoFromRecipe(Recipe recipe, IReadOnlyList<IngredientDto> ingredientDtos)
        {
            RecipeDto recipeDto = _mapper.Map<RecipeDto>(recipe);

            recipeDto.CategoryDto = await _categoryService.GetByIdAsync(recipe.CategoryId) ?? throw new InvalidOperationException();

            recipeDto.IngredientMeasurementDtos.Clear();

            foreach (IngredientMeasurement ingredientMeasurement in recipe.IngredientMeasurements)
            {
                IngredientMeasurementDto ingredientMeasurementDto = _mapper.Map<IngredientMeasurementDto>(ingredientMeasurement);
                ingredientMeasurementDto.IngredientDto = ingredientDtos.FirstOrDefault(ingredientDto => ingredientDto.Id == ingredientMeasurement.IngredientId) ?? throw new InvalidOperationException("");

                recipeDto.IngredientMeasurementDtos.Add(ingredientMeasurementDto);
            }

            return recipeDto;
        }
    }
}