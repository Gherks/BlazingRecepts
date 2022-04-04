using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
    }
}
