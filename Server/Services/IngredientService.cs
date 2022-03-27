using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientCategoryService _ingredientCategoryService;
        private readonly IIngredientRepository _ingredientsRepository;
        private readonly IMapper _mapper;

        public IngredientService(IIngredientCategoryService ingredientCategoryService, IIngredientRepository ingredientsRepository, IMapper mapper)
        {
            _ingredientCategoryService = ingredientCategoryService;
            _ingredientsRepository = ingredientsRepository;
            _mapper = mapper;
        }

        public async Task<bool> AnyAsync(Guid id)
        {
            return await _ingredientsRepository.AnyAsync(id);
        }

        public async Task<bool> AnyAsync(string name)
        {
            return await _ingredientsRepository.AnyAsync(name);
        }

        public async Task<IngredientDto?> GetByIdAsync(Guid id)
        {
            Ingredient? ingredient = await _ingredientsRepository.GetByIdAsync(id);

            if (ingredient != null)
            {
                return _mapper.Map<IngredientDto>(ingredient);
            }

            return null;
        }

        public async Task<IReadOnlyList<IngredientDto>> GetAllAsync()
        {
            IReadOnlyList<Ingredient> ingredients = await _ingredientsRepository.ListAllAsync() ?? new List<Ingredient>();

            return ingredients.Select(ingredient => _mapper.Map<IngredientDto>(ingredient)).ToList();
        }

        public async Task<IReadOnlyList<IngredientCollectionTypeDto>> GetAllSortedAsync()
        {
            IReadOnlyList<IngredientCategoryDto> ingredientCategoryDtos = await _ingredientCategoryService.GetAllAsync();

            List<IngredientCollectionTypeDto> ingredientCollectionTypes = new();

            foreach (IngredientCategoryDto ingredientCategoryDto in ingredientCategoryDtos)
            {
                IngredientCollectionTypeDto ingredientCollectionTypeDto = new();
                ingredientCollectionTypeDto.Name = ingredientCategoryDto.Name;

                ingredientCollectionTypes.Add(ingredientCollectionTypeDto);
            }

            IReadOnlyList<Ingredient> ingredients = await _ingredientsRepository.ListAllAsync() ?? new List<Ingredient>();

            foreach (Ingredient ingredient in ingredients)
            {
                ingredientCollectionTypes[ingredient.IngredientCategory.SortOrder].Ingredients.Add(_mapper.Map<IngredientDto>(ingredient));
            }

            foreach (IngredientCollectionTypeDto ingredientCollectionTypeDto in ingredientCollectionTypes)
            {
                ingredientCollectionTypeDto.Ingredients = ingredientCollectionTypeDto.Ingredients
                    .OrderBy(ingredientDto => ingredientDto.Name)
                    .ToList();
            }

            return ingredientCollectionTypes;
        }

        public async Task<IngredientDto> SaveAsync(IngredientDto ingredientDto)
        {
            Ingredient ingredient = _mapper.Map<Ingredient>(ingredientDto);

            if (ingredient.Id == Guid.Empty)
            {
                ingredient = await _ingredientsRepository.AddAsync(ingredient);
            }
            else
            {
                ingredient = await _ingredientsRepository.UpdateAsync(ingredient);
            }

            return _mapper.Map<IngredientDto>(ingredient);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Ingredient? ingredient = await _ingredientsRepository.GetByIdAsync(id);

            if (ingredient != null)
            {
                return await _ingredientsRepository.DeleteAsync(ingredient);
            }

            return false;
        }
    }
}