using AutoMapper;
using Receptacle.Server.Entities;
using Receptacle.Shared.Dto;

namespace Receptacle.Server.AutoMapperProfiles;

public class WeightedIngredientProfile : Profile
{
    public WeightedIngredientProfile()
    {
        CreateMap<WeightedIngredient, WeightedIngredientDto>().ReverseMap();
    }
}
