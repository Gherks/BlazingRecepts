using AutoMapper;
using Receptacle.Server.Entities;
using Receptacle.Server.Repositories.Interfaces;
using Receptacle.Server.Services.Interfaces;
using Receptacle.Shared.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Receptacle.Server.Services
{
    public class IngredientCategoryService : IIngredientCategoryService
    {
        private readonly IIngredientCategoryRepository _repository;
        private readonly IMapper _mapper;

        public IngredientCategoryService(IIngredientCategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<IngredientCategoryDto>> GetAllAsync()
        {
            IReadOnlyList<IngredientCategory> entities = await _repository.ListAllAsync();

            List<IngredientCategoryDto> ingredientCategoryDtos = entities.Select(ingredientCategory => _mapper.Map<IngredientCategoryDto>(ingredientCategory)).ToList();

            ingredientCategoryDtos.Sort(new IngredientCategoryComparer());

            return ingredientCategoryDtos;
        }

        public sealed class IngredientCategoryComparer : IComparer<IngredientCategoryDto>
        {
            public int Compare(IngredientCategoryDto first, IngredientCategoryDto second)
            {
                return first.SortOrder > second.SortOrder ? 1 : -1;
            }
        }
    }
}