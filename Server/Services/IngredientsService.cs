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
    public class IngredientsService : IIngredientsService
    {
        private readonly IIngredientsRepository _repository;
        private readonly IIngredientCategoryService _ingredientCategoryService;
        private readonly IMapper _mapper;

        public IngredientsService(IIngredientsRepository repository, IIngredientCategoryService ingredientCategoryService, IMapper mapper)
        {
            _repository = repository;
            _ingredientCategoryService = ingredientCategoryService;
            _mapper = mapper;
        }

        public async Task<IngredientDto> GetByIdAsync(Guid id)
        {
            Ingredient ingredient = await _repository.GetByIdAsync(id);

            if (ingredient != null)
            {
                IReadOnlyList<IngredientCategoryDto> ingredientCategoryDtos = await _ingredientCategoryService.GetAllAsync();

                IngredientDto ingredientDto = _mapper.Map<IngredientDto>(ingredient);
                ingredientDto.Category = ingredientCategoryDtos.FirstOrDefault(ingredientCategory => ingredientCategory.Id == ingredient.IngredientCategoryId);

                return ingredientDto;
            }

            return null;
        }

        public async Task<IReadOnlyList<IngredientDto>> GetAllAsync()
        {
            IReadOnlyList<Ingredient> ingredients = await _repository.ListAllAsync();
            IReadOnlyList<IngredientCategoryDto> ingredientCategoryDtos = await _ingredientCategoryService.GetAllAsync();

            List<IngredientDto> ingredientDtos = new();

            foreach (Ingredient ingredient in ingredients)
            {
                IngredientDto ingredientDto = _mapper.Map<IngredientDto>(ingredient);
                ingredientDto.Category = ingredientCategoryDtos.FirstOrDefault(ingredientCategory => ingredientCategory.Id == ingredient.IngredientCategoryId);

                ingredientDtos.Add(ingredientDto);
            }

            return ingredientDtos;
        }

        public async Task<IngredientDto> SaveAsync(IngredientDto ingredientDto)
        {
            Ingredient ingredient = _mapper.Map<Ingredient>(ingredientDto);

            if (ingredient.Id == Guid.Empty)
            {
                ingredient = await _repository.AddAsync(ingredient);
            }
            else
            {
                ingredient = await _repository.UpdateAsync(ingredient);
            }

            IReadOnlyList<IngredientCategoryDto> ingredientCategoryDtos = await _ingredientCategoryService.GetAllAsync();

            IngredientDto savedIngredientDto = _mapper.Map<IngredientDto>(ingredient);
            savedIngredientDto.Category = ingredientCategoryDtos.FirstOrDefault(ingredientCategory => ingredientCategory.Id == ingredient.IngredientCategoryId);

            return savedIngredientDto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Ingredient ingredient = await _repository.GetByIdAsync(id);

            if (ingredient != null)
            {
                return await _repository.DeleteAsync(ingredient);
            }

            return false;
        }
    }
}