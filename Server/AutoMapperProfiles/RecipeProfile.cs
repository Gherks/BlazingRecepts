using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        CreateMap<Recipe, RecipeDto>()
            .ForMember(recipeDto => recipeDto.IngredientMeasurementDtos,
                options => options.MapFrom(recipe => recipe.IngredientMeasurements));

        CreateMap<RecipeDto, Recipe>()
            .ForMember(recipe => recipe.IngredientMeasurements,
                options => options.MapFrom(recipeDto => recipeDto.IngredientMeasurementDtos))
            .ForMember(recipe => recipe.CategoryId,
                options => options.MapFrom(recipeDto => recipeDto.CategoryDto.Id));
    }
}
