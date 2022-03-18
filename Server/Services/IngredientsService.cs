using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services
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

        public async Task<IngredientDto?> GetByIdAsync(Guid id)
        {
            Ingredient? ingredient = await _repository.GetByIdAsync(id);

            if (ingredient != null)
            {
                return _mapper.Map<IngredientDto>(ingredient);
            }

            return null;
        }

        public async Task<IReadOnlyList<IngredientDto>> GetAllAsync()
        {
            IReadOnlyList<Ingredient> ingredients = await _repository.ListAllAsync();

            return ingredients.Select(ingredient => _mapper.Map<IngredientDto>(ingredient)).ToList();
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

            return _mapper.Map<IngredientDto>(ingredient);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Ingredient? ingredient = await _repository.GetByIdAsync(id);

            if (ingredient != null)
            {
                return await _repository.DeleteAsync(ingredient);
            }

            return false;
        }
    }
}