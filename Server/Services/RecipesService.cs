using AutoMapper;
using Receptacle.Server.Entities;
using Receptacle.Server.Repositories.Interfaces;
using Receptacle.Server.Services.Interfaces;
using Receptacle.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Receptacle.Server.Services
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

        public async Task<RecipeDto> GetByIdAsync(Guid id)
        {
            Recipe recipe = await _repository.GetByIdAsync(id);

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
            Recipe recipe = await _repository.GetByIdAsync(id);

            if (recipe != null)
            {
                return await _repository.DeleteAsync(recipe);
            }

            return false;
        }
    }
}