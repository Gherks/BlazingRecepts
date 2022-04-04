using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository ingredientCategoryRepository, IMapper mapper)
        {
            _categoryRepository = ingredientCategoryRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllOfTypeAsync(CategoryType categoryType)
        {
            IReadOnlyList<Category> categories = await _categoryRepository.ListAllOfTypeAsync(categoryType) ?? new List<Category>();

            List<CategoryDto> categoryDtos = categories.Select(ingredientCategory => _mapper.Map<CategoryDto>(ingredientCategory)).ToList();

            categoryDtos.Sort(new IngredientCategoryComparer());

            return categoryDtos;
        }

        public sealed class IngredientCategoryComparer : IComparer<CategoryDto>
        {
            public int Compare(CategoryDto? first, CategoryDto? second)
            {
                return first?.SortOrder > second?.SortOrder ? 1 : -1;
            }
        }
    }
}