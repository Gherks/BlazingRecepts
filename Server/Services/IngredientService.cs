using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Serilog;
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

        public async Task<IReadOnlyList<IngredientDto>?> GetAllAsync()
        {
            IReadOnlyList<Ingredient>? ingredients = await _ingredientsRepository.ListAllAsync();

            if (ingredients == null)
            {
                const string errorMessage = "Failed because fetched ingredient list is null.";
                Log.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return ingredients.Select(ingredient => _mapper.Map<IngredientDto>(ingredient)).ToList();
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