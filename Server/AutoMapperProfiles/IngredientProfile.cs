using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>();

        CreateMap<IngredientDto, Ingredient>()
            .ForMember(ingredient => ingredient.CategoryId,
                options => options.MapFrom(ingredientDto => ingredientDto.CategoryDto.Id));
    }
}
