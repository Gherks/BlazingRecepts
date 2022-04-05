using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class IngredientMeasurementProfile : Profile
{
    public IngredientMeasurementProfile()
    {
        CreateMap<IngredientMeasurement, IngredientMeasurementDto>();

        CreateMap<IngredientMeasurementDto, IngredientMeasurement>()
            .ForMember(ingredientMeasurement => ingredientMeasurement.IngredientId,
                options => options.MapFrom(ingredientMeasurementDto => ingredientMeasurementDto.IngredientDto.Id));
    }
}
