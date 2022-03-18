using AutoMapper;
using Receptacle.Server.Entities;
using Receptacle.Shared.Dto;

namespace Receptacle.Server.AutoMapperProfiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>();
        CreateMap<IngredientDto, Ingredient>()
            .ForMember(ingredient => ingredient.IngredientCategoryId,
                options => options.MapFrom(ingredientDto => ingredientDto.Category.Id));
    }
}
