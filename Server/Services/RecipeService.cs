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
        private readonly IMapper _mapper;

        public RecipeService(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
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
                return _mapper.Map<RecipeDto>(recipe);
            }

            return null;
        }

        public async Task<RecipeDto?> GetByNameAsync(string name)
        {
            Recipe? recipe = await _recipeRepository.GetByNameAsync(name);

            if (recipe != null)
            {
                return _mapper.Map<RecipeDto>(recipe);
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

            return recipes.Select(recipe => _mapper.Map<RecipeDto>(recipe)).ToList();
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

            return _mapper.Map<RecipeDto>(recipe);
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
    }
}