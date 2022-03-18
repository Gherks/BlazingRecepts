﻿using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class IngredientCategoryProfile : Profile
{
    public IngredientCategoryProfile()
    {
        CreateMap<IngredientCategory, IngredientCategoryDto>().ReverseMap();
    }
}
