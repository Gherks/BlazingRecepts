using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly ICategoryService _ingredientCategoryService;
        private readonly IIngredientRepository _ingredientsRepository;
        private readonly IMapper _mapper;

        public IngredientService(ICategoryService ingredientCategoryService, IIngredientRepository ingredientsRepository, IMapper mapper)
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
            IReadOnlyList<CategoryDto> categoryDtos = await _ingredientCategoryService.GetAllOfTypeAsync(CategoryType.Ingredient);

            List<IngredientCollectionTypeDto> ingredientCollectionTypes = new();

            foreach (CategoryDto CategoryDto in categoryDtos)
            {
                IngredientCollectionTypeDto ingredientCollectionTypeDto = new();
                ingredientCollectionTypeDto.Name = CategoryDto.Name;

                ingredientCollectionTypes.Add(ingredientCollectionTypeDto);
            }

            IReadOnlyList<Ingredient> ingredients = await _ingredientsRepository.ListAllAsync() ?? new List<Ingredient>();

            foreach (Ingredient ingredient in ingredients)
            {
                ingredientCollectionTypes[ingredient.Category.SortOrder].Ingredients.Add(_mapper.Map<IngredientDto>(ingredient));
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