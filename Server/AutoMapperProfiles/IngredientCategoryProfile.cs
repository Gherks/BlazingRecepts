using AutoMapper;
using Receptacle.Server.Entities;
using Receptacle.Shared.Dto;

namespace Receptacle.Server.AutoMapperProfiles;

public class IngredientCategoryProfile : Profile
{
    public IngredientCategoryProfile()
    {
        CreateMap<IngredientCategory, IngredientCategoryDto>().ReverseMap();
    }
}
