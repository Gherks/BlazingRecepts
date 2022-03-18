using AutoMapper;
using Receptacle.Server.Entities;
using Receptacle.Shared.Dto;

namespace Receptacle.Server.AutoMapperProfiles;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        CreateMap<Recipe, RecipeDto>().ReverseMap();
    }
}
