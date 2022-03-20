using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services
{
    public class IngredientCategoryService : IIngredientCategoryService
    {
        private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
        private readonly IMapper _mapper;

        public IngredientCategoryService(IIngredientCategoryRepository ingredientCategoryRepository, IMapper mapper)
        {
            _ingredientCategoryRepository = ingredientCategoryRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<IngredientCategoryDto>> GetAllAsync()
        {
            IReadOnlyList<IngredientCategory> entities = await _ingredientCategoryRepository.ListAllAsync() ?? new List<IngredientCategory>();

            List<IngredientCategoryDto> ingredientCategoryDtos = entities.Select(ingredientCategory => _mapper.Map<IngredientCategoryDto>(ingredientCategory)).ToList();

            ingredientCategoryDtos.Sort(new IngredientCategoryComparer());

            return ingredientCategoryDtos;
        }

        public sealed class IngredientCategoryComparer : IComparer<IngredientCategoryDto>
        {
            public int Compare(IngredientCategoryDto? first, IngredientCategoryDto? second)
            {
                return first?.SortOrder > second?.SortOrder ? 1 : -1;
            }
        }
    }
}