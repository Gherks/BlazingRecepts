using BlazingRecept.Contract;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Mappers;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        Category? category = await _categoryRepository.GetByIdAsync(id);

        if (category != null)
        {
            return EntityMapper.ToDto(category);
        }

        return null;
    }

    public async Task<IReadOnlyList<CategoryDto>?> GetAllOfTypeAsync(CategoryType categoryType)
    {
        IReadOnlyList<Category>? categories = await _categoryRepository.ListAllOfTypeAsync(categoryType);

        Contracts.LogAndThrowWhenNull(categories, $"Fetched category list from repository was null, tried to fetch all of category type: {categoryType}");

        List<CategoryDto> categoryDtos = categories.Select(ingredientCategory => EntityMapper.ToDto(ingredientCategory)).ToList();

        categoryDtos.Sort((first, second) => first.SortOrder > second.SortOrder ? 1 : -1);

        return categoryDtos;
    }
}
