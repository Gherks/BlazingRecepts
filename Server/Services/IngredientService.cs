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
        private readonly IIngredientRepository _ingredientsRepository;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public IngredientService(IIngredientRepository ingredientsRepository, ICategoryService ingredientCategoryService, IMapper mapper)
        {
            _ingredientsRepository = ingredientsRepository;
            _categoryService = ingredientCategoryService;
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
                IngredientDto ingredientDto = _mapper.Map<IngredientDto>(ingredient);

                ingredientDto.CategoryDto = await _categoryService.GetByIdAsync(ingredient.CategoryId) ?? throw new InvalidOperationException();

                return ingredientDto;
            }

            return null;
        }

        public async Task<IReadOnlyList<IngredientDto>> GetAllAsync()
        {
            IReadOnlyList<CategoryDto> categoryDtos = await _categoryService.GetAllOfTypeAsync(CategoryType.Ingredient);

            IReadOnlyList<Ingredient> ingredients = await _ingredientsRepository.ListAllAsync() ?? new List<Ingredient>();

            List<IngredientDto> ingredientDtos = new List<IngredientDto>();

            foreach (Ingredient ingredient in ingredients)
            {
                ingredientDtos.Add(LoadIngredientDtoFromIngredient(ingredient, categoryDtos));
            }

            return ingredientDtos;
        }

        public async Task<IReadOnlyList<IngredientCollectionTypeDto>> GetAllSortedAsync()
        {
            IReadOnlyList<CategoryDto> categoryDtos = await _categoryService.GetAllOfTypeAsync(CategoryType.Ingredient);

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
                IngredientDto ingredientDto = LoadIngredientDtoFromIngredient(ingredient, categoryDtos);

                ingredientCollectionTypes[ingredientDto.CategoryDto.SortOrder].Ingredients.Add(ingredientDto);
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

            IReadOnlyList<CategoryDto> categoryDtos = await _categoryService.GetAllOfTypeAsync(CategoryType.Ingredient);

            return LoadIngredientDtoFromIngredient(ingredient, categoryDtos);
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

        private IngredientDto LoadIngredientDtoFromIngredient(Ingredient ingredient, IReadOnlyList<CategoryDto> categoryDtos)
        {
            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(ingredient);

            ingredientDto.CategoryDto = categoryDtos.FirstOrDefault(category => category.Id == ingredient.CategoryId) ?? throw new InvalidOperationException();

            return ingredientDto;
        }
    }
}