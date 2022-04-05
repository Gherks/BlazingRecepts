﻿using AutoMapper;
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

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            Category? category = await _categoryRepository.GetByIdAsync(id);

            if (category != null)
            {
                return _mapper.Map<CategoryDto>(category);
            }

            return null;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllOfTypeAsync(CategoryType categoryType)
        {
            IReadOnlyList<Category> categories = await _categoryRepository.ListAllOfTypeAsync(categoryType) ?? new List<Category>();

            List<CategoryDto> categoryDtos = categories.Select(ingredientCategory => _mapper.Map<CategoryDto>(ingredientCategory)).ToList();

            categoryDtos.Sort(new CategoryComparer());

            return categoryDtos;
        }

        public sealed class CategoryComparer : IComparer<CategoryDto>
        {
            public int Compare(CategoryDto? first, CategoryDto? second)
            {
                return first?.SortOrder > second?.SortOrder ? 1 : -1;
            }
        }
    }
}