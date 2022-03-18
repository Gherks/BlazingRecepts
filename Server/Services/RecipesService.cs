using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services
{
    public class RecipesService : IRecipesService
    {
        private readonly IRecipesRepository _repository;
        private readonly IMapper _mapper;

        public RecipesService(IRecipesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RecipeDto?> GetByIdAsync(Guid id)
        {
            Recipe? recipe = await _repository.GetByIdAsync(id);

            if (recipe != null)
            {
                return _mapper.Map<RecipeDto>(recipe);
            }

            return null;
        }

        public async Task<IReadOnlyList<RecipeDto>> GetAllAsync()
        {
            IReadOnlyList<Recipe> entities = await _repository.ListAllAsync();

            return entities.Select(recipe => _mapper.Map<RecipeDto>(recipe)).ToArray();
        }

        public async Task<RecipeDto> SaveAsync(RecipeDto recipeDto)
        {
            Recipe recipe = _mapper.Map<Recipe>(recipeDto);

            if (recipe.Id == Guid.Empty)
            {
                recipe = await _repository.AddAsync(recipe);
            }
            else
            {
                await _repository.UpdateAsync(recipe);
            }

            return _mapper.Map<RecipeDto>(recipe);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Recipe? recipe = await _repository.GetByIdAsync(id);

            if (recipe != null)
            {
                return await _repository.DeleteAsync(recipe);
            }

            return false;
        }
    }
}