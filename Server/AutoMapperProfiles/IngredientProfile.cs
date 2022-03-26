using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>()
            .ForMember(ingredientDto => ingredientDto.CategoryDto,
                options => options.MapFrom(ingredient => ingredient.IngredientCategory));

        CreateMap<IngredientDto, Ingredient>()
            .ForMember(ingredient => ingredient.IngredientCategoryId,
                options => options.MapFrom(ingredientDto => ingredientDto.CategoryDto.Id))
            .ForMember(ingredient => ingredient.IngredientCategory,
                options => options.MapFrom(ingredientDto => ingredientDto.CategoryDto));
    }
}
