using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class IngredientMeasurementProfile : Profile
{
    public IngredientMeasurementProfile()
    {
        CreateMap<IngredientMeasurement, IngredientMeasurementDto>().ReverseMap();

        CreateMap<IngredientMeasurement, IngredientMeasurementDto>()
            .ForMember(ingredientMeasurementDto => ingredientMeasurementDto.IngredientDto,
                options => options.MapFrom(ingredientMeasurement => ingredientMeasurement.Ingredient));

        CreateMap<IngredientMeasurementDto, IngredientMeasurement>()
            .ForMember(ingredientMeasurement => ingredientMeasurement.IngredientId,
                options => options.MapFrom(ingredientMeasurementDto => ingredientMeasurementDto.IngredientDto.Id))
            .ForMember(ingredientMeasurement => ingredientMeasurement.Ingredient,
                options => options.MapFrom(ingredientMeasurementDto => ingredientMeasurementDto.IngredientDto));
    }
}
