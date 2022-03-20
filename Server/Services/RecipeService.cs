using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipesRepository _recipesRepository;
        private readonly IMapper _mapper;

        public RecipeService(IRecipesRepository recipesRepository, IMapper mapper)
        {
            _recipesRepository = recipesRepository;
            _mapper = mapper;
        }

        public async Task<RecipeDto?> GetByIdAsync(Guid id)
        {
            Recipe? recipe = await _recipesRepository.GetByIdAsync(id);

            if (recipe != null)
            {
                return _mapper.Map<RecipeDto>(recipe);
            }

            return null;
        }

        public async Task<IReadOnlyList<RecipeDto>> GetAllAsync()
        {
            IReadOnlyList<Recipe> entities = await _recipesRepository.ListAllAsync() ?? new List<Recipe>();

            return entities.Select(recipe => _mapper.Map<RecipeDto>(recipe)).ToArray();
        }

        public async Task<RecipeDto> SaveAsync(RecipeDto recipeDto)
        {
            Recipe recipe = _mapper.Map<Recipe>(recipeDto);

            if (recipe.Id == Guid.Empty)
            {
                recipe = await _recipesRepository.AddAsync(recipe);
            }
            else
            {
                await _recipesRepository.UpdateAsync(recipe);
            }

            return _mapper.Map<RecipeDto>(recipe);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Recipe? recipe = await _recipesRepository.GetByIdAsync(id);

            if (recipe != null)
            {
                await _recipesRepository.DeleteAsync(recipe);
            }

            return false;
        }
    }
}