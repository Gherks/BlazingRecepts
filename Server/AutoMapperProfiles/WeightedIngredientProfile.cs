using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class WeightedIngredientProfile : Profile
{
    public WeightedIngredientProfile()
    {
        CreateMap<WeightedIngredient, WeightedIngredientDto>().ReverseMap();
    }
}
